.globl main

label1:
	ori		$s0,	$zero,	10
	ori		$s1,	$zero,	'a'
main:
	add		$t0,	$s0,	$s1
	j		main+8

# .data
# .byte 0x1000