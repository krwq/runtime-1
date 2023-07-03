#include <stdio.h>
#include <openssl/evp.h>
#include <openssl/provider.h>
#include <openssl/store.h>
#include <openssl/err.h>
#include <openssl/self_test.h>

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

void print_hex(unsigned char *data, size_t length) {
    for (size_t i = 0; i < length; i++) {
        printf("%02x", data[i]);
    }
}

int do_sha256(unsigned char* data, int dataLen, unsigned char** hashOut) {
    int hashLen = SHA256_DIGEST_LENGTH;
    unsigned char* hash = OPENSSL_malloc(hashLen);

    EVP_MD_CTX *mdctx = EVP_MD_CTX_new();
    const EVP_MD *md = EVP_sha256();

    EVP_DigestInit_ex(mdctx, md, NULL);
    EVP_DigestUpdate(mdctx, data, dataLen);
    EVP_DigestFinal_ex(mdctx, hash, NULL);

    *hashOut = hash;
    return hashLen;
}

int self_test_callback(const OSSL_PARAM params[], char* arg)
{
    printf("self_test_callback called: %s\n", arg);
    return 1;
}

int is_provider_enabled_by_default(const char* name) {
    OSSL_LIB_CTX* lib_ctx = OSSL_LIB_CTX_new();
    int enabled = OSSL_PROVIDER_available(NULL, name);
    OSSL_LIB_CTX_free(lib_ctx);
    return enabled;
}

struct check_already_present_cbdata_st {
    int found;
    const char* name;
};

#define check_already_present_cbdata struct check_already_present_cbdata_st

int check_already_present(OSSL_PROVIDER* provider, void* cbdata) {
    const char* provider_name = OSSL_PROVIDER_get0_name(provider);

    check_already_present_cbdata* data = (check_already_present_cbdata*)cbdata;
    //printf("check_already_present(%s, %s)\n", provider_name, data->name);
    if (strcmp(provider_name, data->name) == 0) {
        data->found = 1;
    }

    return 1;
}

int check_already_present_in_new_context(const char* provider_name) {
    check_already_present_cbdata cbdata;
    cbdata.found = 0;
    cbdata.name = provider_name;
    OSSL_LIB_CTX* lib_ctx = OSSL_LIB_CTX_new();
    OSSL_PROVIDER_do_all(lib_ctx, check_already_present, &cbdata);
    OSSL_LIB_CTX_free(lib_ctx);
    return cbdata.found;
}

int print_all_providers_cb(OSSL_PROVIDER* provider, void* cbdata) {
    printf("provider: %s\n", OSSL_PROVIDER_get0_name(provider));
    return 1;
}

int load_all_providers_cb(OSSL_PROVIDER* provider, void* cbdata) {
    OSSL_LIB_CTX* lib_ctx = (OSSL_LIB_CTX*)cbdata;
    const char* provider_name = OSSL_PROVIDER_get0_name(provider);
    //if (!check_already_present_in_new_context(provider_name)) {
        printf("Loading provider: %s\n", provider_name);
        OSSL_PROVIDER_load(lib_ctx, provider_name);
    //}
    //else {
    //     printf("Provider should already be loaded: %s\n", provider_name);
    //}

    return 1;
}

int main() {
    //OPENSSL_init_crypto(OPENSSL_INIT_LOAD_CONFIG, NULL);
    EVP_PKEY *pkey = NULL;
    EVP_PKEY_CTX *ctx = NULL;
    OSSL_LIB_CTX* libCtx = OSSL_LIB_CTX_new(); //OSSL_LIB_CTX_get0_global_default();//OSSL_LIB_CTX_new_child();// 
    OSSL_PROVIDER *prov = NULL;
    OSSL_STORE_CTX *store_ctx = NULL;
    OSSL_STORE_INFO *info = NULL;
    unsigned char *sig = NULL;
    size_t siglen = 1337;

    unsigned char data[] = "testdata\0";  // Your arbitrary data
    int dataLen = strlen((char*)data) + 1;
    unsigned char* hash;

    // warning: following code is causing tpm2 provider code to never return any results (OSSL_STORE_open will always return NULL)
    // int providerAvailable = OSSL_PROVIDER_available(libCtx, "tpm2");
    // printf("OSSL_PROVIDER_available(tpm2)=%d\n", providerAvailable);

    // warning: as above
    // int providerAvailable = OSSL_PROVIDER_available(libCtx, "tpm3");
    // printf("OSSL_PROVIDER_available(tpm3)=%d\n", providerAvailable);

    OSSL_PROVIDER_do_all(NULL, load_all_providers_cb, libCtx);

    // this works
    // int providerAvailable = is_provider_enabled_by_default("tpm2");
    // printf("is_provider_enabled_by_default(tpm2)=%d\n", providerAvailable);
    // so apparently is_provider_enabled_by_default is meaningless and you still need to load

    // Load the TPM2 provider
    // if (providerAvailable) {
    //     printf("libCtx providers:\n");
    //     OSSL_PROVIDER_do_all(libCtx, print_all_providers_cb, NULL);
    //     printf("NULL ctx providers:\n");
    //     OSSL_PROVIDER_do_all(NULL, print_all_providers_cb, NULL);
    //     printf("---\n");
    // }

    // prov = OSSL_PROVIDER_load(libCtx, "tpm2");
    // if (prov == NULL) {
    //     fprintf(stderr, "Failed to load TPM2 provider\n");
    //     return 1;
    // }
    // else {
    //     const char* cbarg = "test";
    //     OSSL_SELF_TEST_set_callback(libCtx, (OSSL_CALLBACK*)self_test_callback, (void*)cbarg);
    //     if (OSSL_PROVIDER_self_test(prov) != 1) {
    //         fprintf(stderr, "TPM2 provider self test failed\n");
    //         return 1;
    //     }
    // }

    //providerAvailable = OSSL_PROVIDER_available(libCtx, "tpm2");
    //printf("OSSL_PROVIDER_available(tpm2)=%d\n", providerAvailable);

    // Open the store
    printf("openning the store\n");
    store_ctx = OSSL_STORE_open_ex("handle:0x81000004", libCtx, /* propq: */ "provider=tpm2" /*"provider=tpm2"*/, NULL, NULL, NULL, NULL, NULL); //OSSL_STORE_open("handle:0x81000004", NULL, NULL, NULL, NULL);
    if (store_ctx == NULL) {
        fprintf(stderr, "Failed to open store\n");
        return 1;
    }

    // Retrieve each key from the store and check if it's suitable for signing
    int key_index = 0;
    while (!OSSL_STORE_eof(store_ctx) && (info = OSSL_STORE_load(store_ctx)) != NULL) {
        printf("Something found in OSSL_STORE\n");
        if (OSSL_STORE_error(store_ctx)) {
            fprintf(stderr, "OSSL_STORE_error returned 1\n");
            ERR_print_errors_fp(stderr);

            // per OpenSSL docs:
            // Note that it may still be meaningful to try and load more objects
            if (OSSL_STORE_eof(store_ctx))
            {
                return 1;
            }

            continue;
        }

        int store_type = OSSL_STORE_INFO_get_type(info);
        printf("Found key of type %s and name %s\n", OSSL_STORE_INFO_type_string(store_type), OSSL_STORE_INFO_get0_NAME(info));
        printf("Description %s\n", OSSL_STORE_INFO_get1_NAME_description(info));
        if (store_type == OSSL_STORE_INFO_PKEY) {
            pkey = OSSL_STORE_INFO_get1_PKEY(info);
            if (pkey != NULL) {
                // Check if the key is suitable for signing
                ctx = EVP_PKEY_CTX_new(pkey, NULL);//EVP_PKEY_CTX_new_from_pkey(libCtx, pkey, NULL);//
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
        if (OSSL_STORE_error(store_ctx)) {
            fprintf(stderr, "OSSL_STORE_error returned 1 and there was no suitable key for signing\n");
            ERR_print_errors_fp(stderr);
            
            return 1;
        }

        fprintf(stderr, "Failed to find a suitable key for signing\n");
        return 1;
    }

    print_key_type(ctx);

    // for some reason when this is executed before we call OSSL_PROVIDER_load on same OSSL_LIB_CTX
    // it causes OSSL_STORE_load to not find any keys
    // if we reorder or use OSSL_LIB_CTX_new and EVP_PKEY_CTX_new_from_pkey rather than default NULL and EVP_PKEY_CTX_new
    // it works fine.
    // this shows a problem that we need to keep two handles alive while our approved API
    // returns only single handle.
    int hashLen = do_sha256(data, dataLen, &hash);

    // Determine the size of the signature
    printf("hashLen=%d\n", hashLen);
    if (EVP_PKEY_sign(ctx, NULL, &siglen, hash, hashLen) <= 0) {
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

    printf("EVP_PKEY_sign\n");
    // Perform the signing operation
    if (EVP_PKEY_sign(ctx, sig, &siglen, hash, hashLen) <= 0) {
        ERR_print_errors_fp(stderr);
        fprintf(stderr, "Failed to sign data\n");
        return 1;
    }

    printf("Seems all is good!\n");
    printf("Hash:\n");
    print_hex(hash, hashLen);
    printf("\nSignature\n");
    print_hex(sig, siglen);
    printf("\n");
    // At this point, 'sig' contains the signature and 'siglen' is its length

    BIO* pubKeyBio = BIO_new_file("testprovider.exported.pub", "w");
    if (pubKeyBio == NULL) {
        printf("Failed to open testprovider.exported.pub file\n");
        return 1;
    }

    if (PEM_write_bio_PUBKEY(pubKeyBio, pkey) != 1) {
        fprintf(stderr, "Failed to write public key to file\n");
        return 1;
    }

    // Clean up
    OPENSSL_free(sig);
    EVP_PKEY_CTX_free(ctx);
    //EVP_MD_CTX_free(mdctx);
    EVP_PKEY_free(pkey);
    OSSL_STORE_INFO_free(info);
    OSSL_STORE_close(store_ctx);
    OSSL_PROVIDER_unload(prov);

    return 0;
}
