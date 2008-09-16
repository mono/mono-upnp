ASSEMBLY_NAME = Mono.Upnp.Client
ASSEMBLY = $(ASSEMBLY_NAME).dll

MONO_UPNP_CLIENT_SRC =  \
	Mono.Upnp.Internal/Helper.cs \
	Mono.Upnp.Internal/Log.cs \
	Mono.Upnp.Internal/Protocol.cs \
	Mono.Upnp.Internal/SoapAdapter.cs \
	Mono.Upnp.Internal/UpnpDeserializationException.cs \
	Mono.Upnp.Control/Action.cs \
  Mono.Upnp.Control/AllowedValueRange.cs \
	Mono.Upnp.Control/Argument.cs \
	Mono.Upnp.Control/ArgumentDirection.cs \
	Mono.Upnp.Control/StateVariable.cs \
	Mono.Upnp.Control/UpnpControlException.cs \
	Mono.Upnp.Control/UpnpControlExceptionStatus.cs \
	Mono.Upnp/Client.cs \
	Mono.Upnp/Device.cs \
	Mono.Upnp/DeviceType.cs \
	Mono.Upnp/Icon.cs \
	Mono.Upnp/ReadOnlyDictionary.cs \
	Mono.Upnp/Root.cs \
	Mono.Upnp/Service.cs \
	Mono.Upnp/ServiceType.cs \
	Mono.Upnp/TypeInfo.cs \
	Mono.Upnp/UpnpArgs.cs \
	Mono.Upnp/UpnpException.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(MONO_UPNP_CLIENT_SRC) $(POLICY_ASSEMBLIES)
	$(MCS) -debug -keyfile:mono-upnp-client.snk -target:library -out:$@ $(MONO_UPNP_CLIENT_SRC)

install-data-local: gac-install

uninstall-local: gac-uninstall

include $(top_srcdir)/src/Mono.Upnp/Makefile.include

EXTRA_DIST = $(MONO_SSDP_SRC) $(ASSEMBLY_NAME).csproj mono-upnp-client.snk
CLEANFILES = *.dll *.mdb $(POLICY_ASSEMBLIES) $(POLICY_CONFIGS)
DISTCLEANFILES = *.pidb
MAINTAINERCLEANFILES = Makefile.in

