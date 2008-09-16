SUBDIRS = \
	Mono.Ssdp \
	Mono.Ssdp.ConsoleClient

pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = mono-ssdp.pc

uninstall-hook:
	if test $$(find $(pkglibdir) | wc -l) -eq 1; then \
		rmdir $(pkglibdir); \
	fi

EXTRA_DIST = \
	mono-ssdp.pc.in \
	policy.config.in

CLEANFILES = \
	*.dll

DISTCLEANFILES = \
	mono-ssdp.pc

MAINTAINERCLEANFILES = \
	Makefile.in

