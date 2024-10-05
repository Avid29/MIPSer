# Adam Dernis 2023

# Features:
# ------------------
# Label usage:	Entry only
# Macros:		Yes
# Jumps:		No
# Branches:		No
# Memory:		No
# Syscalls:		No
#

.globl main

TEST = 10

main:
	ori		$s0,	$zero,	TEST
	ori		$s1,	$zero,	main
	add		$t0,	$s0,	$s1
