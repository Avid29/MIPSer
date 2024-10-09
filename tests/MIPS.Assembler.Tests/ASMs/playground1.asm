# .globl main

main:
	ori		$s0,	$zero,	10
	ori		$s1,	$zero,	'a'
	add		$t0,	$s0,	$s1
	j		main+8