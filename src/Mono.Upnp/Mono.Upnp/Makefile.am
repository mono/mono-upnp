
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug -define:DEBUG "-define:DEBUG,TRACE" "-keyfile:$(srcdir)/mono-upnp.snk"
ASSEMBLY = ../../../bin/Mono.Upnp.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = ../../../bin/Mono.Ssdp.dll
BUILD_DIR = ../../../bin

MONO_UPNP_DLL_MDB_SOURCE=../../../bin/Mono.Upnp.dll.mdb
MONO_UPNP_DLL_MDB=$(BUILD_DIR)/Mono.Upnp.dll.mdb
MONO_SSDP_DLL_SOURCE=../../../bin/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB_SOURCE=../../../bin/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-keyfile:$(srcdir)/mono-upnp.snk"
ASSEMBLY = ../../../bin/Mono.Upnp.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../../bin/Mono.Ssdp.dll
BUILD_DIR = ../../../bin

MONO_UPNP_DLL_MDB=
MONO_SSDP_DLL_SOURCE=../../../bin/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONO_UPNP_DLL_MDB) \
	$(MONO_SSDP_DLL) \
	$(MONO_SSDP_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MONO_UPNP_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES =  \
	AssemblyInfo.cs \
	Mono.Upnp.Control/AllowedValueRange.cs \
	Mono.Upnp.Control/Argument.cs \
	Mono.Upnp.Control/ArgumentDirection.cs \
	Mono.Upnp.Control/IMap.cs \
	Mono.Upnp.Control/ServiceAction.cs \
	Mono.Upnp.Control/ServiceActionExecutor.cs \
	Mono.Upnp.Control/ServiceController.cs \
	Mono.Upnp.Control/StateVariable.cs \
	Mono.Upnp.Control/StateVariableChangedArgs.cs \
	Mono.Upnp.Control/StateVariableEventer.cs \
	Mono.Upnp.Control/StateVariableOptions.cs \
	Mono.Upnp.Control/UpnpActionAttribute.cs \
	Mono.Upnp.Control/UpnpArgumentAttribute.cs \
	Mono.Upnp.Control/UpnpControlException.cs \
	Mono.Upnp.Control/UpnpEnumAttribute.cs \
	Mono.Upnp.Control/UpnpError.cs \
	Mono.Upnp.Control/UpnpRelatedStateVariableAttribute.cs \
	Mono.Upnp.Control/UpnpServiceDefinitionException.cs \
	Mono.Upnp.Control/UpnpStateVariableAttribute.cs \
	Mono.Upnp.Internal/Arguments.cs \
	Mono.Upnp.Internal/CollectionMap.cs \
	Mono.Upnp.Internal/ControlClient.cs \
	Mono.Upnp.Internal/ControlServer.cs \
	Mono.Upnp.Internal/DataServer.cs \
	Mono.Upnp.Internal/EmptyDictionary.cs \
	Mono.Upnp.Internal/EventClient.cs \
	Mono.Upnp.Internal/EventServer.cs \
	Mono.Upnp.Internal/Func.cs \
	Mono.Upnp.Internal/Helper.cs \
	Mono.Upnp.Internal/IMappable.cs \
	Mono.Upnp.Internal/Log.cs \
	Mono.Upnp.Internal/Map.cs \
	Mono.Upnp.Internal/Protocol.cs \
	Mono.Upnp.Internal/ServiceControllerBuilder.cs \
	Mono.Upnp.Internal/SoapEnvelope.cs \
	Mono.Upnp.Internal/SoapFault.cs \
	Mono.Upnp.Internal/SoapHeader.cs \
	Mono.Upnp.Internal/TimeoutDispatcher.cs \
	Mono.Upnp.Internal/UpnpServer.cs \
	Mono.Upnp.Internal/XmlShell.cs \
	Mono.Upnp.Xml.Compilation/Compiler.cs \
	Mono.Upnp.Xml.Compilation/DelegateDeserializationCompiler.cs \
	Mono.Upnp.Xml.Compilation/DelegateSerializationCompiler.cs \
	Mono.Upnp.Xml.Compilation/DeserializationCompiler.cs \
	Mono.Upnp.Xml.Compilation/DeserializationCompilerProvider.cs \
	Mono.Upnp.Xml.Compilation/Deserializer.cs \
	Mono.Upnp.Xml.Compilation/ObjectDeserializer.cs \
	Mono.Upnp.Xml.Compilation/SerializationCompiler.cs \
	Mono.Upnp.Xml.Compilation/SerializationCompilerProvider.cs \
	Mono.Upnp.Xml.Compilation/Serializer.cs \
	Mono.Upnp.Xml/DoNotDeserializeAttribute.cs \
	Mono.Upnp.Xml/DoNotSerializeAttribute.cs \
	Mono.Upnp.Xml/IXmlDeserializable.cs \
	Mono.Upnp.Xml/IXmlDeserializer.cs \
	Mono.Upnp.Xml/IXmlSerializable.cs \
	Mono.Upnp.Xml/XmlArrayAttribute.cs \
	Mono.Upnp.Xml/XmlArrayItemAttribute.cs \
	Mono.Upnp.Xml/XmlAttributeAttribute.cs \
	Mono.Upnp.Xml/XmlAutomatable.cs \
	Mono.Upnp.Xml/XmlDeclarationType.cs \
	Mono.Upnp.Xml/XmlDeserializable.cs \
	Mono.Upnp.Xml/XmlDeserializationContext.cs \
	Mono.Upnp.Xml/XmlDeserializer.cs \
	Mono.Upnp.Xml/XmlElementAttribute.cs \
	Mono.Upnp.Xml/XmlEnumAttribute.cs \
	Mono.Upnp.Xml/XmlFlagAttribute.cs \
	Mono.Upnp.Xml/XmlNamespaceAttribute.cs \
	Mono.Upnp.Xml/XmlSerializable.cs \
	Mono.Upnp.Xml/XmlSerializationContext.cs \
	Mono.Upnp.Xml/XmlSerializationOptions.cs \
	Mono.Upnp.Xml/XmlSerializer.cs \
	Mono.Upnp.Xml/XmlTypeAttribute.cs \
	Mono.Upnp.Xml/XmlValueAttribute.cs \
	Mono.Upnp/Client.cs \
	Mono.Upnp/Description.cs \
	Mono.Upnp/Deserializer.cs \
	Mono.Upnp/DeserializerProducer.cs \
	Mono.Upnp/Device.cs \
	Mono.Upnp/DeviceAnnouncement.cs \
	Mono.Upnp/DeviceOptions.cs \
	Mono.Upnp/DeviceType.cs \
	Mono.Upnp/DisposedEventArgs.cs \
	Mono.Upnp/Icon.cs \
	Mono.Upnp/Root.cs \
	Mono.Upnp/Server.cs \
	Mono.Upnp/Service.cs \
	Mono.Upnp/ServiceAnnouncement.cs \
	Mono.Upnp/ServiceType.cs \
	Mono.Upnp/SpecVersion.cs \
	Mono.Upnp/TypeInfo.cs \
	Mono.Upnp/UpnpDeserializationException.cs \
	Mono.Upnp/UpnpEventArgs.cs \
	Mono.Upnp/UpnpException.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	Mono.Upnp.Xml.Compilation \
	mono-upnp.snk \
	mono.upnp.pc.in 

REFERENCES =  \
	System \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

MONO_SSDP_DLL = $(BUILD_DIR)/Mono.Ssdp.dll
MONO_UPNP_PC = $(BUILD_DIR)/mono.upnp.pc

$(eval $(call emit-deploy-wrapper,MONO_UPNP_PC,mono.upnp.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
