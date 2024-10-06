# Adam Dernis 2023

# Features:
# ------------------
# Label usage:	Entry only
# Macros:		No
# Jumps:		No
# Branches:		No
# Memory:		No
# Syscalls:		No
#

.globl main

main:
	ori		$s0,	$zero,	10
	ori		$s1,	$zero,	'a'
	add		$t0,	$s0,	$s1
