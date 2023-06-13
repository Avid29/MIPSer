# Adam Dernis 2023

#
# This program is meant to test some basic failures.
#
# Features:
# ------------------
# Label usage:	Entry only
# Macros:		No
# Jumps:		Yes, with links
# Branches:		No
# Memory:		No
# Syscalls:		Yes
#

main:
	ori		$v0,	$zero		# Missing argument
	ori		$a0,	$zero, 0
	xkcd	$t0,	$a0, 0		# No such instruction
	syscall

	lui		$at,	10($t0)		# Wrong argument
	sw	    $v0,	176($at)
	lui		$at,	4096
	sw		$v1,	180($at)
	lw		$a0,	0($sp)
	lw		$a1,	4(sp)		# Not a register
	lw		$a2,	8($sp)		

	jal    1048576
	
	sll    $zero, $hero, 0		# Not a valid register
	add    $a0, $v0, $zero
	ori    $v0, $zero, 17
	syscall