# Adam Dernis 2023

#
# This program is meant to test the most basic assembling steps.
#
# Features:
# ------------------
# Label usage:	Entry only
# Macros:		No
# Jumps:		No
# Branches:		No
# Memory:		No
# Syscalls:		No
#

main:
	ori		$s0,	$zero,	10
	ori		$s1,	$zero,	10
	add		$t0,	$s0,	$s1
