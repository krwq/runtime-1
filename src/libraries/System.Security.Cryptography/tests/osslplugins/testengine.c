#include <stdio.h>

#define OPENSSL_SUPPRESS_DEPRECATED

#include <openssl/evp.h>
#include <openssl/engine.h>
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

int main() {
    ENGINE *e;
    EVP_PKEY *pkey = NULL;
    EVP_PKEY_CTX *ctx = NULL;

    OpenSSL_add_all_algorithms();

    e = ENGINE_by_id("tpm2tss");
    if (!e) {
        fprintf(stderr, "Error loading engine\n");
        return 1;
    }

    if (!ENGINE_init(e)) {
        fprintf(stderr, "Error initializing engine\n");
        ENGINE_free(e);
        return 1;
    }

    pkey = ENGINE_load_private_key(e, "0x81000007", NULL, NULL);
    if (!pkey) {
        fprintf(stderr, "Error loading private key\n");
        ENGINE_finish(e);
        ENGINE_free(e);
        return 1;
    }

    ctx = EVP_PKEY_CTX_new(pkey, NULL);
    int sign_init_result = EVP_PKEY_sign_init(ctx);
    if (sign_init_result <= 0) {
        fprintf(stderr, "EVP_PKEY_sign_init failed: %d\n", sign_init_result);
        return 1;
    }

    // Set the hash function to SHA256
    if (EVP_PKEY_CTX_set_signature_md(ctx, EVP_sha256()) <= 0) {
        ERR_print_errors_fp(stderr);
        fprintf(stderr, "Failed to set signature hash function to SHA256\n");
        return 1;
    }

    unsigned char *sig = NULL;
    size_t siglen = 1337;

    unsigned char data[] = "testdata\0";  // Your arbitrary data
    int dataLen = strlen((char*)data) + 1;
    unsigned char* hash;

    int hashLen = do_sha256(data, dataLen, &hash);

    // Determine the size of the signature
    printf("hashLen=%d\n", hashLen);
    if (EVP_PKEY_sign(ctx, NULL, &siglen, hash, hashLen) <= 0) {
        ERR_print_errors_fp(stderr);
        fprintf(stderr, "Failed to determine signature size\n");
        return 1;
    }

    printf("EVP_PKEY_sign finished sanity error check\n", (int)siglen);
    ERR_print_errors_fp(stderr);

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

    if (siglen == 0)
    {
        printf("Signature length is 0\n");
        ERR_print_errors_fp(stderr);
        fprintf(stderr, "Failed to sign data\n");
        return 1;
    }

    printf("Seems no errors!\n");
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

    OPENSSL_free(sig);
    EVP_PKEY_CTX_free(ctx);
    EVP_PKEY_free(pkey);


    return 0;
}
