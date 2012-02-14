
EXTRA_DIST = \
	expansions.m4 \
	README.md

#Warning: This is an automatically generated file, do not edit!
if ENABLE_DEBUG
 SUBDIRS =  src/Mono.Ssdp/Mono.Ssdp src/Mono.Upnp src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1 tests
endif
if ENABLE_RELEASE
 SUBDIRS =  src/Mono.Ssdp/Mono.Ssdp src/Mono.Upnp src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1 tests
endif

.PHONY: test

test: all
	(cd bin; $(NUNIT_CMD) *.Tests.dll)
