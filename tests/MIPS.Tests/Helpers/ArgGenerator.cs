﻿// Adam Dernis 2025

using MIPS.Models.Instructions.Enums;
using MIPS.Models.Instructions.Enums.Operations;
using MIPS.Models.Instructions.Enums.Registers;
using MIPS.Models.Instructions.Enums.SpecialFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MIPS.Tests.Helpers;

public class ArgGenerator
{
    /// <remarks>
    /// Safe constrains the value to within the 16 bit range.
    /// For good testing purposes, this shouldn't always be used as the masking should fix overflowing immediates.
    /// </remarks>
    public static short RandomImmediate(bool safe = true) => (short)Random.Shared.Next(safe ? short.MaxValue : int.MaxValue);

    public static byte RandomShift(bool safe = true) => (byte)Random.Shared.Next(safe ? (1 << 5)-1 : int.MaxValue);

    public static int RandomOffset(bool safe = true) => Random.Shared.Next(safe ? (1 << 17)-1 : int.MaxValue) & ~0b11;

    public static uint RandomAddress(bool safe = true) => (uint)Random.Shared.Next(safe ? (1 << 26)-1 : int.MaxValue) & ~(uint)0b11;

    public static Register RandomRegister(bool safe = true) => (Register)Random.Shared.Next(safe ? (int)Register.ReturnAddress : int.MaxValue);

    public static OperationCode RandomOpCode(bool safe = true) => (OperationCode)Random.Shared.Next(safe ? (int)OperationCode.StoreWordCoprocessor3 : int.MaxValue);

    public static FunctionCode RandomFuncCode(bool safe = true) => (FunctionCode)Random.Shared.Next(safe ? (int)FunctionCode.SetLessThanUnsigned : int.MaxValue);

    public static FloatFormat RandomFormat(HashSet<FloatFormat>? set) => set?.ElementAt(Random.Shared.Next(set.Count-1)) ?? FloatFormat.Single;
}
