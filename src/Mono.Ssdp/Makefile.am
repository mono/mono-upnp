ASSEMBLY_NAME = Mono.Ssdp
ASSEMBLY = $(ASSEMBLY_NAME).dll

MONO_SSDP_SRC =  \
	Mono.Ssdp.Internal/AsyncReceiveBuffer.cs \
	Mono.Ssdp.Internal/HttpDatagram.cs \
	Mono.Ssdp.Internal/HttpDatagramType.cs \
	Mono.Ssdp.Internal/Log.cs \
	Mono.Ssdp.Internal/NotifyListener.cs \
	Mono.Ssdp.Internal/Protocol.cs \
	Mono.Ssdp.Internal/ServiceCache.cs \
	Mono.Ssdp.Internal/SsdpSocket.cs \
	Mono.Ssdp.Internal/TimerDispatcher.cs \
	Mono.Ssdp/Browser.cs \
	Mono.Ssdp/BrowseService.cs \
	Mono.Ssdp/Client.cs \
	Mono.Ssdp/MulticastReader.cs \
	Mono.Ssdp/Service.cs \
	Mono.Ssdp/ServiceHandler.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(MONO_SSDP_SRC) $(POLICY_ASSEMBLIES)
	$(MCS) -debug -keyfile:$(top_srcdir)/src/mono-ssdp.snk -target:library -out:$@ $(MONO_SSDP_SRC)

install-data-local: gac-install

uninstall-local: gac-uninstall

include $(top_srcdir)/src/Makefile.include

EXTRA_DIST = $(MONO_SSDP_SRC) $(ASSEMBLY_NAME).csproj
CLEANFILES = *.dll *.mdb $(POLICY_ASSEMBLIES) $(POLICY_CONFIGS)
DISTCLEANFILES = *.pidb
MAINTAINERCLEANFILES = Makefile.in

