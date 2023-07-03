## Testing OpenSSL ENGINE

## Testing OpenSSL Provider

### Testing TPM2 provider

#### Installation

To install TPM2 provider refer to https://github.com/tpm2-software/tpm2-openssl - on Ubuntu as of writing this document following step can be used:

```bash
sudo apt install tpm2-openssl tpm2-tools tpm2-abrmd libtss2-tcti-tabrmd0
```

#### Getting TPM handles

To list all available persistent handles following tpm2-tools command can be used:

```bash
tpm2_getcap handles-persistent
```

Command by default will only list handles but no information about them.
To get information about specific handle:

```bash
tpm2_readpublic -c 0x81000004
```

Apparently that also works:

```bash
tpm2_readpublic -c 0x81000004 -o /tmp/key.pub
```

#### Testing signing with OpenSSL CLI

```bash
# create testdata file with some content
echo 'content' > testdata

# hash data
cat testdata | openssl dgst -sha256 -binary > testdata.dgst

# sign data
openssl pkeyutl -provider tpm2 -inkey handle:0x81000004 -sign -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-1 -pkeyopt digest:sha256 -in testdata.dgst -out testdata.sig

# get public key (PEM)
openssl pkey -provider tpm2 -pubout -in handle:0x81000004 -out testkey.pub

# verify data
openssl pkeyutl -verify -in testdata.dgst -sigfile testdata.sig -inkey testkey.pub -pubin -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-2 -pkeyopt digest:sha256
```

#### Creating keys

To create handles `tpm2_createprimary` and `tpm2_create` can be used. Refer to their respective manuals to get more info (i.e. `man tpm2_createprimary`).

For example primary key can be created using:

```bash
tpm2_createprimary -C o -c primary.ctx -G rsa
```

### scratchpad

```
openssl pkeyutl -provider tpm2 -inkey handle:0x81000002 -sign -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-1 -pkeyopt digest:sha256 -in testdata -out testdata.sig



tpm2_create -G rsa -g sha256 -u key.pub -r key.priv -L policy.txt -a "noda|adminwithpolicy|sign|restricted|fixedtpm|fixedparent|sensitivedataorigin"


tpm2_createprimary -C o -g rsapss-sha256 -G rsa -c primary.ctx
tpm2_create -C primary.ctx -g rsapss-sha256 -G rsa -u key.pub -r key.priv -a "noda|adminwithpolicy|sign|restricted|fixedtpm|fixedparent|sensitivedataorigin"



# this creates a key
tpm2_createprimary -C o -g sha256 -G rsa2048:rsapss:null -c primary.ctx -a 'fixedtpm|fixedparent|sensitivedataorigin|userwithauth|noda|sign'
# | decrypt is needed for ENGINE APIs to work since they use that underneath
tpm2_createprimary -C o -g sha256 -G rsa2048:rsapss:null -c primary.ctx -a 'fixedtpm|fixedparent|sensitivedataorigin|userwithauth|noda'

# Stores and prints the handle
tpm2_evictcontrol -C o -c primary.ctx
# if passed 0x81000000 as last argument it can be forced to use that handle number

# handle should show up here now:
tpm2_getcap handles-persistent

# to print data info about that handle:
tpm2_readpublic -c 0x81000004

# ?? untested ?? to get public key
openssl pkey -provider tpm2 -provider base -inkey handle:0x81000004 -pubout -out testkey.pub
openssl rsa -provider tpm2 -pubout -inkey handle:0x81000004 -out testkey.pub

# test handle with CLI
cat testdata | openssl dgst -sha256 -binary > testdata.dgst
openssl pkeyutl -provider tpm2 -inkey handle:0x81000004 -sign -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-1 -pkeyopt digest:sha256 -in testdata.dgst -out testdata.sig

# not working
tpm2_create -C primary.ctx -g sha256 -G rsa2048:rsapss:null -u key.pub -r key.priv -a 'restricted|fixedtpm|fixedparent|sensitivedataorigin|userwithauth|sign'
```


## Building tpm2-tss-engine

To enable extra logging:

```
export TSS2_LOG=all+TRACE
```

everything as one INSTALL.md page but this worked for me - ./configure produced bunch of warnings treated as errors which following suppresses:

```
./configure CFLAGS='-DOPENSSL_SUPPRESS_DEPRECATED -Wno-incompatible-pointer-types -Wno-discarded-qualifiers'
```

Check if works after:
```
openssl engine -t -c tpm2tss
```

 that prints following for me:

 ```
(tpm2tss) TPM2-TSS engine for OpenSSL
 [RSA, RAND]
     [ available ]
4007A032E27F0000:error:1280006A:DSO support routines:dlfcn_bind_func:could not bind to the requested symbol name:../crypto/dso/dso_dlfcn.c:188:symname(EVP_PKEY_base_id): /usr/lib/x86_64-linux-gnu/engines-3/tpm2tss.so: undefined symbol: EVP_PKEY_base_id
4007A032E27F0000:error:1280006A:DSO support routines:DSO_bind_func:could not bind to the requested symbol name:../crypto/dso/dso_lib.c:176:
 ```

 Per https://github.com/openssl/openssl/issues/17962 those errors can be ignored.


### Signing with CLI

```
# working provider stuff
openssl pkeyutl -provider tpm2 -inkey handle:0x81000004 -sign -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-1 -pkeyopt digest:sha256 -in testdata.dgst -out testdata.sig


-keyform engine

openssl pkeyutl -engine tpm2tss -inkey handle:0x81000004 -sign -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-1 -pkeyopt digest:sha256 -in testdata.dgst -out testdata.sig



openssl dgst -engine tpm2tss -keyform engine -sha256 -sign 0x81000004 -out testdata.sig testdata



openssl pkeyutl -engine tpm2tss -keyform engine -inkey 0x81000005 -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-1 -pkeyopt digest:sha256 -in testdata.dgst -out testdata.sig
openssl pkeyutl -engine tpm2tss -inkey 0x81000006 -sign -keyform engine -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-1 -pkeyopt digest:sha256 -in testdata.dgst -out testdata.sig
```


## tpm2-tss-engine ECDSA

```
tpm2_createprimary -C o -g sha256 -G ecc256:ecdsa-sha256:null -c primary.ctx -a 'fixedtpm|fixedparent|sensitivedataorigin|userwithauth|noda|sign'

# store handle and print it
tpm2_evictcontrol -C o -c primary.ctx
# 0x81000007
```