

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG" "-keyfile:$(srcdir)/mono-ssdp.snk"
ASSEMBLY = ../../../bin/Mono.Ssdp.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../../../bin

MONO_SSDP_DLL_MDB_SOURCE=../../../bin/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-keyfile:$(srcdir)/mono-ssdp.snk"
ASSEMBLY = ../../../bin/Mono.Ssdp.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../../../bin

MONO_SSDP_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONO_SSDP_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MONO_SSDP_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	AssemblyInfo.cs \
	Mono.Ssdp/Announcer.cs \
	Mono.Ssdp/Browser.cs \
	Mono.Ssdp/BrowseService.cs \
	Mono.Ssdp/Client.cs \
	Mono.Ssdp/MulticastReader.cs \
	Mono.Ssdp/Server.cs \
	Mono.Ssdp/Service.cs \
	Mono.Ssdp/ServiceArgs.cs \
	Mono.Ssdp/ServiceHandler.cs \
	Mono.Ssdp.Internal/AsyncReceiveBuffer.cs \
	Mono.Ssdp.Internal/HttpDatagram.cs \
	Mono.Ssdp.Internal/HttpDatagramType.cs \
	Mono.Ssdp.Internal/Log.cs \
	Mono.Ssdp.Internal/NotifyListener.cs \
	Mono.Ssdp.Internal/Protocol.cs \
	Mono.Ssdp.Internal/RequestListener.cs \
	Mono.Ssdp.Internal/ServiceCache.cs \
	Mono.Ssdp.Internal/SsdpSocket.cs \
	Mono.Ssdp.Internal/TimeoutDispatcher.cs \
	Mono.Ssdp.Internal/MulticastSsdpSocket.cs \
	Mono.Ssdp.Internal/NetworkInterfaceInfo.cs \
	Mono.Ssdp/ServiceOperation.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	mono-ssdp.snk \
	mono.ssdp.pc.in 

REFERENCES =  \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

MONO_SSDP_PC = $(BUILD_DIR)/mono.ssdp.pc

$(eval $(call emit-deploy-wrapper,MONO_SSDP_PC,mono.ssdp.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
