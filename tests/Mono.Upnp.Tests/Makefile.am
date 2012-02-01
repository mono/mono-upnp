
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = Mono.Upnp.Tests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../src/Mono.Upnp/Mono.Upnp/Mono.Upnp.dll \
	../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
BUILD_DIR = .

MONO_UPNP_TESTS_DLL_MDB_SOURCE=Mono.Upnp.Tests.dll.mdb
MONO_UPNP_TESTS_DLL_MDB=$(BUILD_DIR)/Mono.Upnp.Tests.dll.mdb
MONO_UPNP_DLL_SOURCE=../../src/Mono.Upnp/Mono.Upnp/Mono.Upnp.dll
MONO_SSDP_DLL_SOURCE=../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB_SOURCE=../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb
MONO_UPNP_DLL_MDB_SOURCE=../../src/Mono.Upnp/Mono.Upnp/Mono.Upnp.dll.mdb
MONO_UPNP_DLL_MDB=$(BUILD_DIR)/Mono.Upnp.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = Mono.Upnp.Tests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../src/Mono.Upnp/Mono.Upnp/Mono.Upnp.dll \
	../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
BUILD_DIR = .

MONO_UPNP_TESTS_DLL_MDB=
MONO_UPNP_DLL_SOURCE=../../src/Mono.Upnp/Mono.Upnp/Mono.Upnp.dll
MONO_SSDP_DLL_SOURCE=../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB=
MONO_UPNP_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONO_UPNP_TESTS_DLL_MDB) \
	$(MONO_UPNP_DLL) \
	$(MONO_SSDP_DLL) \
	$(MONO_SSDP_DLL_MDB) \
	$(MONO_UPNP_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) 

FILES =  \
	ClientTests.cs \
	DeviceDescriptionTests.cs \
	DummyDeserializer.cs \
	DummyDeserializerFactory.cs \
	DummyIcon.cs \
	DummyRoot.cs \
	DummyService.cs \
	DummyServiceAction.cs \
	DummyServiceController.cs \
	DummyStateVariable.cs \
	DummyStateVariableEventer.cs \
	ServerTests.cs \
	ServiceDescriptionTests.cs \
	ServiceTests.cs \
	TypeInfoTests.cs \
	Xml.cs \
	XmlDeserializationTests.cs \
	XmlSerializationTests.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll \
	../../src/Mono.Upnp/Mono.Upnp/Mono.Upnp.dll \
	-pkg:nunit \
	System \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) 

include $(top_srcdir)/Makefile.include

MONO_UPNP_DLL = $(BUILD_DIR)/Mono.Upnp.dll
MONO_SSDP_DLL = $(BUILD_DIR)/Mono.Ssdp.dll

$(eval $(call emit-deploy-target,MONO_UPNP_DLL))
$(eval $(call emit-deploy-target,MONO_SSDP_DLL))
$(eval $(call emit-deploy-target,MONO_SSDP_DLL_MDB))
$(eval $(call emit-deploy-target,MONO_UPNP_DLL_MDB))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
