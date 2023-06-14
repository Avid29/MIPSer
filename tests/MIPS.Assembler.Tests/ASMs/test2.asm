# Adam Dernis 2023

# Features:
# ------------------
# Label usage:	Yes
# Macros:		Yes
# Jumps:		No
# Branches:		No
# Memory:		No
# Syscalls:		No
#

TEST = 1

main:
	ori		$s0,	$zero,	10
	ori		$s1,	$zero,	main
	add		$t0,	$s0,	$s1
