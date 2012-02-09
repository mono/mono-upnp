
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug -define:DEBUG "-define:DEBUG,TRACE" "-keyfile:$(srcdir)/mono-upnp-dcp-mediaserver1.snk"
ASSEMBLY = ../../../../bin/Mono.Upnp.Dcp.MediaServer1.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../../../bin/Mono.Upnp.dll
BUILD_DIR = ../../../../bin

MONO_UPNP_DCP_MEDIASERVER1_DLL_MDB_SOURCE=../../../../bin/Mono.Upnp.Dcp.MediaServer1.dll.mdb
MONO_UPNP_DCP_MEDIASERVER1_DLL_MDB=$(BUILD_DIR)/Mono.Upnp.Dcp.MediaServer1.dll.mdb
MONO_UPNP_DLL_SOURCE=../../../../bin/Mono.Upnp.dll
MONO_SSDP_DLL_SOURCE=../../../../bin/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB_SOURCE=../../../../bin/Mono.Ssdp.dll.mdb
MONO_SSDP_DLL_MDB=$(BUILD_DIR)/Mono.Ssdp.dll.mdb
MONO_UPNP_DLL_MDB_SOURCE=../../../../bin/Mono.Upnp.dll.mdb
MONO_UPNP_DLL_MDB=$(BUILD_DIR)/Mono.Upnp.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-keyfile:$(srcdir)/mono-upnp-dcp-mediaserver1.snk"
ASSEMBLY = ../../../../bin/Mono.Upnp.Dcp.MediaServer1.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../../../bin/Mono.Upnp.dll
BUILD_DIR = ../../../../bin

MONO_UPNP_DCP_MEDIASERVER1_DLL_MDB=
MONO_UPNP_DLL_SOURCE=../../../../bin/Mono.Upnp.dll
MONO_SSDP_DLL_SOURCE=../../../../bin/Mono.Ssdp.dll
MONO_SSDP_DLL_MDB=
MONO_UPNP_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONO_UPNP_DCP_MEDIASERVER1_DLL_MDB) \
	$(MONO_UPNP_DLL) \
	$(MONO_SSDP_DLL) \
	$(MONO_SSDP_DLL_MDB) \
	$(MONO_UPNP_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MONO_UPNP_DCP_MEDIASERVER1_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	AssemblyInfo.cs \
	Mono.Upnp.Dcp.MediaServer1/DiscoveryEventArgs.cs \
	Mono.Upnp.Dcp.MediaServer1/TransferStatus.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/BrowseFlag.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Class.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ClassManager.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ClassNameAttribute.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ClassReference.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Container.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ContentDirectoryClient.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ContentDirectoryController.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/DeserializationException.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Item.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Object.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Resolution.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Resource.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Results.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ResultsSettings.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Schemas.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/WriteStatus.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ObjectBasedContentDirectory.cs \
	Mono.Upnp.Dcp.MediaServer1/MediaServer.cs \
	Mono.Upnp.Dcp.MediaServer1.ConnectionManager1/ConnectionManagerController.cs \
	Mono.Upnp.Dcp.MediaServer1.ConnectionManager1/ConnectionManagerClient.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/Album.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/AudioBook.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/AudioBroadcast.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/AudioItem.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/Genre.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/ImageItem.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/Movie.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MovieGenre.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MusicAlbum.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MusicArtist.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MusicGenre.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MusicTrack.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MusicVideoClip.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/Person.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/PersonWithRole.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/Photo.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/PhotoAlbum.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/PlaylistContainer.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/PlaylistItem.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/StorageFolder.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/StorageSystem.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/StorageVolume.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/TextItem.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/VideoBroadcast.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/VideoItem.cs \
	Mono.Upnp.Dcp.MediaServer1.ConnectionManager1/ConnectionManager.cs \
	Mono.Upnp.Dcp.MediaServer1.ConnectionManager1/ConnectionStatus.cs \
	Mono.Upnp.Dcp.MediaServer1.ConnectionManager1/Direction.cs \
	Mono.Upnp.Dcp.MediaServer1.Internal/Arguments.cs \
	Mono.Upnp.Dcp.MediaServer1.Internal/StripedCollection.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/UpdateDelegateSerializationCompiler.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/UpdateContext.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/UpdateTextWriter.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/UpdateXmlSerializer.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/FilteringDelegateSerializationCompiler.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/FilteringContext.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/QueryVisitor.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Query.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/QueryParser.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/QueryParsingException.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ObjectQueryVisitor.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/PropertyName.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ObjectQueryContext.cs \
	Mono.Upnp.Dcp.MediaServer1.ConnectionManager1/ProtocolInfo.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/StorageVolumeOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/StorageSystemOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/StorageFolderOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/AlbumOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/AudioBookOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/AudioBroadcastOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/AudioItemOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/GenreOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/ImageItemOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MovieOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MusicAlbumOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MusicArtistOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MusicTrackOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/MusicVideoClipOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/PersonOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/PhotoOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/PlaylistContainerOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/PlaylistItemOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/TextItemOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/VideoBroadcastOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV/VideoItemOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ContainerOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ItemOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ObjectOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ResourceOptions.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Error.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/LocalContentDirectory.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/RemoteContentDirectory.cs \
	Mono.Upnp.Dcp.MediaServer1.Internal/SearchResults.cs \
	Mono.Upnp.Dcp.MediaServer1.Internal/BrowseResults.cs \
	Mono.Upnp.Dcp.MediaServer1.Internal/QueryStringifier.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/ContentDirectory.cs \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1/Reference.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/VirtualDelegateSerializationCompiler.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/VirtualContext.cs \
	Mono.Upnp.Dcp.MediaServer1.Xml/Override.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	Mono.Upnp.Dcp.MediaServer1.ConnectionManager1 \
	Mono.Upnp.Dcp.MediaServer1.Internal \
	Mono.Upnp.Dcp.MediaServer1.Xml \
	Mono.Upnp.Dcp.MediaServer1 \
	Mono.Upnp.Dcp.MediaServer1.ContentDirectory1 \
	mono-upnp-dcp-mediaserver1.snk \
	mono.upnp.dcp.mediaserver1.pc.in 

REFERENCES =  \
	System \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

MONO_UPNP_DLL = $(BUILD_DIR)/Mono.Upnp.dll
MONO_SSDP_DLL = $(BUILD_DIR)/Mono.Ssdp.dll
MONO_UPNP_DCP_MEDIASERVER1_PC = $(BUILD_DIR)/mono.upnp.dcp.mediaserver1.pc

$(eval $(call emit-deploy-wrapper,MONO_UPNP_DCP_MEDIASERVER1_PC,mono.upnp.dcp.mediaserver1.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
