#include <stdio.h>
#include <openssl/evp.h>
#include <openssl/provider.h>
#include <openssl/store.h>
#include <openssl/err.h>

void print_key_type(EVP_PKEY_CTX *ctx) {
    EVP_PKEY *pkey = EVP_PKEY_CTX_get0_pkey(ctx);
    if (pkey == NULL) {
        printf("No key associated with context\n");
        return;
    }

    int key_type = EVP_PKEY_base_id(pkey);
    switch (key_type) {
        case EVP_PKEY_RSA:
            printf("Key type: RSA\n");
            break;
        case EVP_PKEY_DSA:
            printf("Key type: DSA\n");
            break;
        case EVP_PKEY_EC:
            printf("Key type: EC\n");
            break;
        // Add more cases as needed
        default:
            printf("Key type: Unknown\n");
            break;
    }
}

int main() {
    EVP_PKEY *pkey = NULL;
    EVP_PKEY_CTX *ctx = NULL;
    OSSL_PROVIDER *prov = NULL;
    OSSL_STORE_CTX *store_ctx = NULL;
    OSSL_STORE_INFO *info = NULL;
    unsigned char *sig = NULL;
    size_t siglen = 1337;
    unsigned char data[] = "testdata";  // Your arbitrary data

    // Load the TPM2 provider
    prov = OSSL_PROVIDER_load(NULL, "tpm2");
    if (prov == NULL) {
        fprintf(stderr, "Failed to load TPM2 provider\n");
        return 1;
    }

    // Open the store
    store_ctx = OSSL_STORE_open("handle:0x81000002", NULL, NULL, NULL, NULL);
    if (store_ctx == NULL) {
        fprintf(stderr, "Failed to open store\n");
        return 1;
    }

    // Retrieve each key from the store and check if it's suitable for signing
    int key_index = 0;
    while (!OSSL_STORE_eof(store_ctx) && (info = OSSL_STORE_load(store_ctx)) != NULL) {
        int store_type = OSSL_STORE_INFO_get_type(info);
        printf("Found key of type %s and name %s\n", OSSL_STORE_INFO_type_string(store_type), OSSL_STORE_INFO_get0_NAME(info));
        printf("Description %s\n", OSSL_STORE_INFO_get1_NAME_description(info));
        if (store_type == OSSL_STORE_INFO_PKEY) {
            pkey = OSSL_STORE_INFO_get1_PKEY(info);
            if (pkey != NULL) {
                // Check if the key is suitable for signing
                ctx = EVP_PKEY_CTX_new(pkey, NULL);
                if (ctx != NULL) {
                    int sign_init_result = EVP_PKEY_sign_init(ctx);
                    printf("Key index: %d, sign_init_result: %d\n", key_index, sign_init_result);
                    if (sign_init_result > 0) {
                        // Set the signature algorithm to RSASSA-PSS
                        if (EVP_PKEY_CTX_set_rsa_padding(ctx, RSA_PKCS1_PSS_PADDING) <= 0) {
                            ERR_print_errors_fp(stderr);
                            fprintf(stderr, "Failed to set RSA padding to PSS\n");
                            return 1;
                        }

                        // Set the hash function to SHA256
                        if (EVP_PKEY_CTX_set_signature_md(ctx, EVP_sha256()) <= 0) {
                            ERR_print_errors_fp(stderr);
                            fprintf(stderr, "Failed to set signature hash function to SHA256\n");
                            return 1;
                        }

                        // This key is suitable for signing
                        break;
                    }
                } else {
                    printf("Failed to create context for key index: %d\n", key_index);
                }
                EVP_PKEY_CTX_free(ctx);
                ctx = NULL;
                EVP_PKEY_free(pkey);
                pkey = NULL;
            } else {
                printf("Failed to get key from store info at index: %d\n", key_index);
            }
        } else {
            printf("Key index: %d is not suitable for signing because store type is %d\n", key_index, store_type);
        }

        OSSL_STORE_INFO_free(info);
        info = NULL;
        key_index++;
    }

    if (pkey == NULL) {
        fprintf(stderr, "Failed to find a suitable key for signing\n");
        return 1;
    }

    print_key_type(ctx);

    // Determine the size of the signature
    if (EVP_PKEY_sign(ctx, NULL, &siglen, data, sizeof(data)) <= 0) {
        ERR_print_errors_fp(stderr);
        fprintf(stderr, "Failed to determine signature size\n");
        return 1;
    }

    printf("EVP_PKEY_sign returned that signature is %d bytes long\n", (int)siglen);
    // Allocate memory for the signature
    sig = OPENSSL_malloc(siglen);
    if (sig == NULL) {
        fprintf(stderr, "Failed to allocate memory for signature\n");
        return 1;
    }

    // Perform the signing operation
    if (EVP_PKEY_sign(ctx, sig, &siglen, data, sizeof(data)) <= 0) {
        ERR_print_errors_fp(stderr);
        fprintf(stderr, "Failed to sign data\n");
        return 1;
    }

    // At this point, 'sig' contains the signature and 'siglen' is its length

    // Clean up
    OPENSSL_free(sig);
    EVP_PKEY_CTX_free(ctx);
    EVP_PKEY_free(pkey);
    OSSL_STORE_INFO_free(info);
    OSSL_STORE_close(store_ctx);
    OSSL_PROVIDER_unload(prov);

    return 0;
}
