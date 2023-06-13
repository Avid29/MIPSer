# Adam Dernis 2023

#
# This program is meant to test some basic failures.
#
# Features:
# ------------------
# Label usage:	Entry only
# Macros:		No
# Jumps:		No
# Branches:		No
# Memory:		No
#

main:
	ori		$s0,	$zero			# Missing argument
	ori		$s1,	$zero,	10		# Clean
	xkcd	$t0,	$s0,	$s1		# Invalid instruction
