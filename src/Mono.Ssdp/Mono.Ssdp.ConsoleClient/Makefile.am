ASSEMBLY_NAME = Mono.Ssdp.ConsoleClient
ASSEMBLY = $(ASSEMBLY_NAME).exe

assemblydir = $(libdir)/mono-ssdp
assembly_SCRIPTS = $(ASSEMBLY) $(ASSEMBLY).mdb

wrapperdir = $(bindir)
wrapper_SCRIPTS = mssdp-client

MONO_SSDP_CONSOLECLIENT_SRC = Mono.Ssdp.ConsoleClient/ConsoleClient.cs 

all: $(ASSEMBLY)

$(ASSEMBLY): $(top_builddir)/src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll $(MONO_SSDP_CONSOLECLIENT_SRC)
	$(MCS) -debug -out:$@ -r:$< $(MONO_SSDP_CONSOLECLIENT_SRC)

EXTRA_DIST = $(MONO_SSDP_CONSOLECLIENT_SRC) $(ASSEMBLY_NAME).csproj
CLEANFILES = *.dll *.exe *.mdb
DISTCLEANFILES = *.pidb
MAINTAINERCLEANFILES = Makefile.in

