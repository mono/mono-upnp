dnl Warning: This is an automatically generated file, do not edit!
dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([Mono.Upnp], [0.1.2])
AM_INIT_AUTOMAKE([foreign tar-ustar])
AM_MAINTAINER_MODE

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

SHAMROCK_EXPAND_LIBDIR
SHAMROCK_EXPAND_BINDIR
SHAMROCK_EXPAND_DATADIR

AC_PROG_INSTALL

AC_PATH_PROG(MCS, mcs, no)
if test "x$MCS" = "xno"; then
        AC_MSG_ERROR([mcs Not found])
fi


AC_ARG_ENABLE(debug,
	AC_HELP_STRING([--enable-debug],
		[Use 'DEBUG' Configuration [default=YES]]),
		enable_debug=yes, enable_debug=no)
AM_CONDITIONAL(ENABLE_DEBUG, test x$enable_debug = xyes)
if test "x$enable_debug" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
AC_ARG_ENABLE(release,
	AC_HELP_STRING([--enable-release],
		[Use 'RELEASE' Configuration [default=NO]]),
		enable_release=yes, enable_release=no)
AM_CONDITIONAL(ENABLE_RELEASE, test x$enable_release = xyes)
if test "x$enable_release" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
if test -z "$CONFIG_REQUESTED" ; then
	AM_CONDITIONAL(ENABLE_DEBUG, true)
	enable_debug=yes
fi


dnl package checks, common for all configs
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0])
PKG_CHECK_MODULES([GLIB_SHARP_20], [glib-sharp-2.0])
PKG_CHECK_MODULES([GLADE_SHARP_20], [glade-sharp-2.0])
PKG_CHECK_MODULES([MONO_ADDINS], [mono-addins])
PKG_CHECK_MODULES([TAGLIB_SHARP], [taglib-sharp])

AC_ARG_ENABLE(tests,
	AC_HELP_STRING([--enable-tests],
		[Build nunit tests [default=YES]]),
		[], enable_tests=yes)
	AM_CONDITIONAL(ENABLE_TESTS, test x$enable_tests = xyes)
	if test "x$enable_tests" = "xyes" ; then
		PKG_CHECK_MODULES([NUNIT], [nunit])
		AC_PATH_PROG(NUNIT_CMD, nunit-console2, nunit-console)
		AC_SUBST(NUNIT_CMD)
	fi

AC_CONFIG_FILES([
src/Mono.Ssdp/Mono.Ssdp/mono.ssdp.pc
src/Mono.Ssdp/Mono.Ssdp/Makefile
src/Mono.Upnp/Mono.Upnp/mono.upnp.pc
src/Mono.Upnp/Mono.Upnp/Makefile
src/Mono.Upnp/Mono.Upnp.GtkClient/mono-upnp-gtk
src/Mono.Upnp/Mono.Upnp.GtkClient/Makefile
src/Mono.Upnp/Makefile
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1/mono.upnp.dcp.mediaserver1.pc
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1/Makefile
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MSMediaReceiverRegistrar1/mono.upnp.dcp.msmediareceiverregistrar1.pc
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MSMediaReceiverRegistrar1/Makefile
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.FileSystem/mono.upnp.dcp.mediaserver1.filesystem.pc
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.FileSystem/Makefile
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.FileSystem.ConsoleServer/mono-upnp-simple-media-server
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.FileSystem.ConsoleServer/Makefile
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.GtkClient/mono.upnp.dcp.mediaserver1.gtkclient.pc
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Mono.Upnp.Dcp.MediaServer1.GtkClient/Makefile
src/Mono.Upnp.Dcp/Mono.Upnp.Dcp.MediaServer1/Makefile
tests/Mono.Ssdp.Tests/Makefile
tests/Mono.Upnp.Tests/Makefile
tests/Mono.Upnp.Dcp.MediaServer1.Tests/Makefile
tests/Mono.Upnp.Dcp.MediaServer1.FileSystem.Tests/Makefile
tests/Makefile
Makefile

])

AC_OUTPUT
