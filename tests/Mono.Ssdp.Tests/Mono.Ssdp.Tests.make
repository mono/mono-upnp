

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = Mono.Ssdp.Tests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
BUILD_DIR = .

MONO_SSDP_TESTS_DLL_MDB_SOURCE=Mono.Ssdp.Tests.dll.mdb
MONO_SSDP_TESTS_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.Tests.dll.mdb
MONO_SSDP_DLL_SOURCE=../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB_SOURCE=../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = Mono.Ssdp.Tests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
BUILD_DIR = .

MONO_SSDP_TESTS_DLL_MDB=
MONO_SSDP_DLL_SOURCE=../../src/Mono.Ssdp/Mono.Ssdp/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONO_SSDP_TESTS_DLL_MDB) \
	$(MONO_SSDP_DLL) \
	$(MONO_SSDP_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) 

FILES = \
	ClientTests.cs \
	ServerTests.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	System \
	$(NUNIT_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) 

include $(top_srcdir)/Makefile.include

MONO_SSDP_DLL = $(BUILD_DIR)/Mono.Ssdp.dll

$(eval $(call emit-deploy-target,MONO_SSDP_DLL))
$(eval $(call emit-deploy-target,MONO_SSDP_DLL_MDB))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
