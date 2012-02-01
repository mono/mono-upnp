
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = ../../../Mono.Upnp/Mono.Upnp.GtkClient/Mono.Upnp.Dcp.MediaServer1.GtkClient.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.dll \
	../../../Mono.Upnp/Mono.Upnp.GtkClient/Mono.Upnp.GtkClient.exe \
	../../../Mono.Upnp/Mono.Upnp/Mono.Upnp.dll
BUILD_DIR = ../../../Mono.Upnp/Mono.Upnp.GtkClient

MONO_UPNP_DCP_MEDIASERVER1_GTKCLIENT_DLL_MDB_SOURCE=../../../Mono.Upnp/Mono.Upnp.GtkClient/Mono.Upnp.Dcp.MediaServer1.GtkClient.dll.mdb
MONO_UPNP_DCP_MEDIASERVER1_GTKCLIENT_DLL_MDB=$(BUILD_DIR)/Mono.Upnp.Dcp.MediaServer1.GtkClient.dll.mdb
MONO_UPNP_DCP_MEDIASERVER1_DLL_SOURCE=../Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.dll
MONO_UPNP_DLL_SOURCE=../../../Mono.Upnp/Mono.Upnp/Mono.Upnp.dll
MONO_SSDP_DLL_SOURCE=../../../Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB_SOURCE=../../../Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb
MONO_UPNP_DLL_MDB_SOURCE=../../../Mono.Upnp/Mono.Upnp/Mono.Upnp.dll.mdb
MONO_UPNP_DLL_MDB=$(BUILD_DIR)/Mono.Upnp.dll.mdb
MONO_UPNP_GTKCLIENT_EXE_SOURCE=../../../Mono.Upnp/Mono.Upnp.GtkClient/Mono.Upnp.GtkClient.exe

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = ../../../Mono.Upnp/Mono.Upnp.GtkClient/Mono.Upnp.Dcp.MediaServer1.GtkClient.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.dll \
	../../../Mono.Upnp/Mono.Upnp.GtkClient/Mono.Upnp.GtkClient.exe \
	../../../Mono.Upnp/Mono.Upnp/Mono.Upnp.dll
BUILD_DIR = ../../../Mono.Upnp/Mono.Upnp.GtkClient

MONO_UPNP_DCP_MEDIASERVER1_GTKCLIENT_DLL_MDB=
MONO_UPNP_DCP_MEDIASERVER1_DLL_SOURCE=../Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.dll
MONO_UPNP_DLL_SOURCE=../../../Mono.Upnp/Mono.Upnp/Mono.Upnp.dll
MONO_SSDP_DLL_SOURCE=../../../Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB=
MONO_UPNP_DLL_MDB=
MONO_UPNP_GTKCLIENT_EXE_SOURCE=../../../Mono.Upnp/Mono.Upnp.GtkClient/Mono.Upnp.GtkClient.exe

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONO_UPNP_DCP_MEDIASERVER1_GTKCLIENT_DLL_MDB) \
	$(MONO_UPNP_DCP_MEDIASERVER1_DLL) \
	$(MONO_UPNP_DLL) \
	$(MONO_SSDP_DLL) \
	$(MONO_SSDP_DLL_MDB) \
	$(MONO_UPNP_DLL_MDB) \
	$(MONO_UPNP_GTKCLIENT_EXE)  

LINUX_PKGCONFIG = \
	$(MONO_UPNP_DCP_MEDIASERVER1_GTKCLIENT_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES =  \
	AssemblyInfo.cs \
	Mono.Upnp.Dcp.MediaServer1.GtkClient/ContentDirectoryInfo.cs \
	Mono.Upnp.Dcp.MediaServer1.GtkClient/ContentDirectoryServiceInfoProvider.cs \
	Mono.Upnp.Dcp.MediaServer1.GtkClient/ObjectInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	Mono.Upnp.Dcp.MediaServer1.GtkClient \
	mono.upnp.dcp.mediaserver1.gtkclient.pc.in 

REFERENCES =  \
	../../../Mono.Upnp/Mono.Upnp.GtkClient/Mono.Upnp.GtkClient.exe \
	../../../Mono.Upnp/Mono.Upnp/Mono.Upnp.dll \
	../Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.dll \
	-pkg:glib-sharp-2.0 \
	-pkg:gtk-sharp-2.0 \
	-pkg:mono-addins \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

MONO_UPNP_DCP_MEDIASERVER1_DLL = $(BUILD_DIR)/Mono.Upnp.Dcp.MediaServer1.dll
MONO_UPNP_DLL = $(BUILD_DIR)/Mono.Upnp.dll
MONO_SSDP_DLL = $(BUILD_DIR)/Mono.Ssdp.dll
MONO_UPNP_GTKCLIENT_EXE = $(BUILD_DIR)/Mono.Upnp.GtkClient.exe
MONO_UPNP_DCP_MEDIASERVER1_GTKCLIENT_PC = $(BUILD_DIR)/mono.upnp.dcp.mediaserver1.gtkclient.pc

$(eval $(call emit-deploy-target,MONO_UPNP_DCP_MEDIASERVER1_DLL))
$(eval $(call emit-deploy-target,MONO_UPNP_DLL))
$(eval $(call emit-deploy-target,MONO_SSDP_DLL))
$(eval $(call emit-deploy-target,MONO_SSDP_DLL_MDB))
$(eval $(call emit-deploy-target,MONO_UPNP_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONO_UPNP_DCP_MEDIASERVER1_GTKCLIENT_PC,mono.upnp.dcp.mediaserver1.gtkclient.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
