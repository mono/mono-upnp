AC_INIT([mono-ssdp], [0.1.0])
AC_CANONICAL_SYSTEM
AC_PREREQ(2.13)
AM_INIT_AUTOMAKE([1.9 dist-bzip2 tar-ustar dist-zip])
AM_MAINTAINER_MODE

API_VERSION="1.0.0.10"
POLICY_VERSIONS="1.0"
AC_SUBST(API_VERSION)
AC_SUBST(POLICY_VERSIONS)

DISTCHECK_CONFIGURE_FLAGS="--disable-docs"
AC_SUBST(DISTCHECK_CONFIGURE_FLAGS)

AC_PROG_INSTALL

SHAMROCK_CHECK_MONO_MODULE(1.1.10)
SHAMROCK_FIND_MONO_2_0_COMPILER
SHAMROCK_FIND_MONO_RUNTIME
SHAMROCK_CHECK_MONODOC
SHAMROCK_CHECK_MONO_2_0_GAC_ASSEMBLIES([
	System
])

AC_PATH_PROG(GACUTIL, gacutil, no)
if test "x$GACUTIL" = "xno"; then
	AC_MSG_ERROR([You need to install gacutil])
fi

AC_PATH_PROG(AL, al2, no)
if test "x$AL" = "xno" ; then
	AC_MSG_ERROR([You need to install al])
fi

GACUTIL_FLAGS='-package $(PACKAGE) -root $(DESTDIR)$(prefix)/lib'
GACUTIL_POLICY_FLAGS='-root $(DESTDIR)$(prefix)/lib'
AC_SUBST(GACUTIL_FLAGS)
AC_SUBST(GACUTIL_POLICY_FLAGS)

SHAMROCK_EXPAND_LIBDIR

AC_OUTPUT([
Makefile
src/MakeFile
src/Mono.Ssdp/MakeFile
src/Mono.Ssdp/policy.config
src/Mono.Ssdp/mono-ssdp.pc
src/Mono.Ssdp/Mono.Ssdp/Makefile
src/Mono.Ssdp/Mono.Ssdp.ConsoleClient/Makefile
src/Mono.Ssdp/Mono.Ssdp.ConsoleClient/mssdp-client
src/Mono.Upnp/MakeFile

docs/Makefile
])

echo "
    Installation Prefix:     ${prefix}
    C# Compiler:             ${MCS}
    Developer Documentation: ${enable_docs}
"

