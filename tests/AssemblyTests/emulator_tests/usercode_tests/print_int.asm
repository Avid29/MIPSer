# Adam Dernis 2024

# Prints the number 42 using a syscall

entry:
	li		$a0,	42
	li		$v0, 1
	syscall
