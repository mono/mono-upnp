AC_DEFUN([SHAMROCK_EXPAND_LIBDIR],
[	
	expanded_libdir=`(
		case $prefix in 
			NONE) prefix=$ac_default_prefix ;; 
			*) ;; 
		esac
		case $exec_prefix in 
			NONE) exec_prefix=$prefix ;; 
			*) ;; 
		esac
		eval echo $libdir
	)`
	AC_SUBST(expanded_libdir)
])

AC_DEFUN([SHAMROCK_FIND_PROGRAM],
[
	AC_PATH_PROG($1, $2, $3)
	AC_SUBST($1)
])

AC_DEFUN([SHAMROCK_FIND_PROGRAM_OR_BAIL],
[
	SHAMROCK_FIND_PROGRAM($1, $2, no)
	if test "x$1" = "xno"; then
		AC_MSG_ERROR([You need to install '$2'])
	fi
])

AC_DEFUN([SHAMROCK_FIND_MONO_1_0_COMPILER],
[
	SHAMROCK_FIND_PROGRAM_OR_BAIL(MCS, mcs)
])

AC_DEFUN([SHAMROCK_FIND_MONO_2_0_COMPILER],
[
	SHAMROCK_FIND_PROGRAM_OR_BAIL(MCS, gmcs)
])

AC_DEFUN([SHAMROCK_FIND_MONO_RUNTIME],
[
	SHAMROCK_FIND_PROGRAM_OR_BAIL(MONO, mono)
])

AC_DEFUN([SHAMROCK_CHECK_MONO_MODULE],
[
	PKG_CHECK_MODULES(MONO_MODULE, mono >= $1)
])

AC_DEFUN([_SHAMROCK_CHECK_MONO_GAC_ASSEMBLIES],
[
	for asm in $(echo "$*" | cut -d, -f2- | sed 's/\,/ /g')
	do
		AC_MSG_CHECKING([for Mono $1 GAC for $asm.dll])
		if test \
			-e "$($PKG_CONFIG --variable=libdir mono)/mono/$1/$asm.dll" -o \
			-e "$($PKG_CONFIG --variable=prefix mono)/lib/mono/$1/$asm.dll"; \
			then \
			AC_MSG_RESULT([found])
		else
			AC_MSG_RESULT([not found])
			AC_MSG_ERROR([missing reqired Mono $1 assembly: $asm.dll])
		fi
	done
])

AC_DEFUN([SHAMROCK_CHECK_MONO_1_0_GAC_ASSEMBLIES],
[
	_SHAMROCK_CHECK_MONO_GAC_ASSEMBLIES(1.0, $*)
])

AC_DEFUN([SHAMROCK_CHECK_MONO_2_0_GAC_ASSEMBLIES],
[
	_SHAMROCK_CHECK_MONO_GAC_ASSEMBLIES(2.0, $*)
])

AC_DEFUN([SHAMROCK_CHECK_MONODOC],
[
	AC_ARG_ENABLE(docs, AC_HELP_STRING([--disable-docs], 
		[Do not build documentation]), , enable_docs=yes)

	if test "x$enable_docs" = "xyes"; then
		AC_PATH_PROG(MONODOCER, monodocer, no)
		if test "x$MONODOCER" = "xno"; then
			AC_MSG_ERROR([You need to install monodoc, or pass --disable-docs to configure to skip documentation installation])
		fi

		AC_PATH_PROG(MDASSEMBLER, mdassembler, no)
		if test "x$MDASSEMBLER" = "xno"; then
			AC_MSG_ERROR([You need to install mdassembler, or pass --disable-docs to configure to skip documentation installation])
		fi

		DOCDIR=`$PKG_CONFIG monodoc --variable=sourcesdir`
		AC_SUBST(DOCDIR)
		AM_CONDITIONAL(BUILD_DOCS, true)
	else
		AC_MSG_NOTICE([not building API documentation])
		AM_CONDITIONAL(BUILD_DOCS, false)
	fi
])

