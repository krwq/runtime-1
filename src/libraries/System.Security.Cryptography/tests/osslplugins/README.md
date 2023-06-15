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

#### Testing signing with OpenSSL CLI

```bash
# create testdata file with some content
echo 'content' > testdata

# hash data
cat testdata | openssl dgst -sha256 -binary > testdata.dgst

# sign data
openssl pkeyutl -provider tpm2 -inkey handle:0x81000004 -sign -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-1 -pkeyopt digest:sha256 -in testdata.dgst -out testdata.sig
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
# | decrypt?
# Stores and prints the handle
tpm2_evictcontrol -C o -c primary.ctx
# if passed 0x81000000 as last argument it can be forced to use that handle number

# handle should show up here now:
tpm2_getcap handles-persistent

# to print data info about that handle:
tpm2_readpublic -c 0x81000004

# ?? untested ?? to get public key
openssl pkey -provider tpm2 -provider base -inkey handle:0x81000004 -pubout -out testkey.pub

# test handle with CLI
cat testdata | openssl dgst -sha256 -binary > testdata.dgst
openssl pkeyutl -provider tpm2 -inkey handle:0x81000004 -sign -pkeyopt rsa_padding_mode:pss -pkeyopt rsa_pss_saltlen:-1 -pkeyopt digest:sha256 -in testdata.dgst -out testdata.sig

# not working
tpm2_create -C primary.ctx -g sha256 -G rsa2048:rsapss:null -u key.pub -r key.priv -a 'restricted|fixedtpm|fixedparent|sensitivedataorigin|userwithauth|sign'
```
