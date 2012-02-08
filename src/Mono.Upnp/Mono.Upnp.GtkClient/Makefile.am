
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = ../../../bin/Mono.Upnp.GtkClient.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	../../../bin/Mono.Upnp.dll
BUILD_DIR = ../../../bin

MONO_UPNP_GTKCLIENT_EXE_MDB_SOURCE=../../../bin/Mono.Upnp.GtkClient.exe.mdb
MONO_UPNP_GTKCLIENT_EXE_MDB=$(BUILD_DIR)/Mono.Upnp.GtkClient.exe.mdb
MONO_UPNP_DLL_SOURCE=../../../bin/Mono.Upnp.dll
MONO_SSDP_DLL_SOURCE=../../../bin/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB_SOURCE=../../../bin/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb
MONO_UPNP_DLL_MDB_SOURCE=../../../bin/Mono.Upnp.dll.mdb
MONO_UPNP_DLL_MDB=$(BUILD_DIR)/Mono.Upnp.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = ../../../bin/Mono.Upnp.GtkClient.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	../../../bin/Mono.Upnp.dll
BUILD_DIR = ../../../bin

MONO_UPNP_GTKCLIENT_EXE_MDB=
MONO_UPNP_DLL_SOURCE=../../../bin/Mono.Upnp.dll
MONO_SSDP_DLL_SOURCE=../../../bin/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB=
MONO_UPNP_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONO_UPNP_GTKCLIENT_EXE_MDB) \
	$(MONO_UPNP_DLL) \
	$(MONO_SSDP_DLL) \
	$(MONO_SSDP_DLL_MDB) \
	$(MONO_UPNP_DLL_MDB)  

BINARIES = \
	$(MONO_UPNP_GTK)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES =  \
	AssemblyInfo.cs \
	gtk-gui/generated.cs \
	gtk-gui/Mono.Upnp.GtkClient.ActionInvocationWindow.cs \
	gtk-gui/Mono.Upnp.GtkClient.DeviceAnnouncementInfo.cs \
	gtk-gui/Mono.Upnp.GtkClient.DeviceDescriptionInfo.cs \
	gtk-gui/Mono.Upnp.GtkClient.LazyDeviceInfo.cs \
	gtk-gui/Mono.Upnp.GtkClient.LazyIcon.cs \
	gtk-gui/Mono.Upnp.GtkClient.LazyServiceInfo.cs \
	gtk-gui/Mono.Upnp.GtkClient.MainWindow.cs \
	gtk-gui/Mono.Upnp.GtkClient.RawXmlInfo.cs \
	gtk-gui/Mono.Upnp.GtkClient.ServiceAnnouncementInfo.cs \
	gtk-gui/Mono.Upnp.GtkClient.ServiceDescriptionInfo.cs \
	gtk-gui/Mono.Upnp.GtkClient.ServiceScpdInfo.cs \
	Mono.Upnp.GtkClient/ActionInvocationWindow.cs \
	Mono.Upnp.GtkClient/DeviceAnnouncementInfo.cs \
	Mono.Upnp.GtkClient/DeviceAnnouncementInfoProvider.cs \
	Mono.Upnp.GtkClient/DeviceDescriptionInfo.cs \
	Mono.Upnp.GtkClient/DeviceDescriptionInfoProvider.cs \
	Mono.Upnp.GtkClient/GtkClient.cs \
	Mono.Upnp.GtkClient/IDeviceInfoProvider.cs \
	Mono.Upnp.GtkClient/IServiceInfoProvider.cs \
	Mono.Upnp.GtkClient/LazyDeviceInfo.cs \
	Mono.Upnp.GtkClient/LazyIcon.cs \
	Mono.Upnp.GtkClient/LazyServiceInfo.cs \
	Mono.Upnp.GtkClient/MainWindow.cs \
	Mono.Upnp.GtkClient/RawDeviceDescriptionInfoProvider.cs \
	Mono.Upnp.GtkClient/RawServiceDescriptionInfoProvider.cs \
	Mono.Upnp.GtkClient/RawServiceScdpInfoProvider.cs \
	Mono.Upnp.GtkClient/RawXmlInfo.cs \
	Mono.Upnp.GtkClient/ServiceAnnouncementInfo.cs \
	Mono.Upnp.GtkClient/ServiceAnnouncementInfoProvider.cs \
	Mono.Upnp.GtkClient/ServiceDescriptionInfo.cs \
	Mono.Upnp.GtkClient/ServiceDescriptionInfoProvider.cs \
	Mono.Upnp.GtkClient/ServiceScpdInfo.cs \
	Mono.Upnp.GtkClient/ServiceScpdInfoProvider.cs 

DATA_FILES = 

RESOURCES = gtk-gui/gui.stetic 

EXTRAS = \
	app.desktop \
	Mono.Upnp.GtkClient \
	mono-upnp-gtk.in 

REFERENCES =  \
	../../../bin/Mono.Upnp.dll \
	Mono.Posix \
	-pkg:glade-sharp-2.0 \
	-pkg:glib-sharp-2.0 \
	-pkg:gtk-sharp-2.0 \
	-pkg:mono-addins \
	System \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

MONO_UPNP_DLL = $(BUILD_DIR)/Mono.Upnp.dll
MONO_SSDP_DLL = $(BUILD_DIR)/Mono.Ssdp.dll
MONO_UPNP_GTK = $(BUILD_DIR)/mono-upnp-gtk

$(eval $(call emit-deploy-wrapper,MONO_UPNP_GTK,mono-upnp-gtk,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
