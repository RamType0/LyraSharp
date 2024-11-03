﻿namespace TensorFlowLite
{
    /// <summary>
    /// The enum for builtin operators.
    /// </summary>
    /// <remarks>
    /// CUSTOM, DELEGATE, and PLACEHOLDER_FOR_GREATER_OP_CODES are 3 special
    /// ops which are not real built-in ops.
    /// </remarks>
    public enum TfLiteBuiltinOperator
    {
        BuiltinAdd = 0,
        BuiltinAveragePool2d = 1,
        BuiltinConcatenation = 2,
        BuiltinConv2d = 3,
        BuiltinDepthwiseConv2d = 4,
        BuiltinDepthToSpace = 5,
        BuiltinDequantize = 6,
        BuiltinEmbeddingLookup = 7,
        BuiltinFloor = 8,
        BuiltinFullyConnected = 9,
        BuiltinHashtableLookup = 10,
        BuiltinL2Normalization = 11,
        BuiltinL2Pool2d = 12,
        BuiltinLocalResponseNormalization = 13,
        BuiltinLogistic = 14,
        BuiltinLshProjection = 15,
        BuiltinLstm = 16,
        BuiltinMaxPool2d = 17,
        BuiltinMul = 18,
        BuiltinRelu = 19,
        BuiltinReluN1To1 = 20,
        BuiltinRelu6 = 21,
        BuiltinReshape = 22,
        BuiltinResizeBilinear = 23,
        BuiltinRnn = 24,
        BuiltinSoftmax = 25,
        BuiltinSpaceToDepth = 26,
        BuiltinSvdf = 27,
        BuiltinTanh = 28,
        BuiltinConcatEmbeddings = 29,
        BuiltinSkipGram = 30,
        BuiltinCall = 31,
        BuiltinCustom = 32,
        BuiltinEmbeddingLookupSparse = 33,
        BuiltinPad = 34,
        BuiltinUnidirectionalSequenceRnn = 35,
        BuiltinGather = 36,
        BuiltinBatchToSpaceNd = 37,
        BuiltinSpaceToBatchNd = 38,
        BuiltinTranspose = 39,
        BuiltinMean = 40,
        BuiltinSub = 41,
        BuiltinDiv = 42,
        BuiltinSqueeze = 43,
        BuiltinUnidirectionalSequenceLstm = 44,
        BuiltinStridedSlice = 45,
        BuiltinBidirectionalSequenceRnn = 46,
        BuiltinExp = 47,
        BuiltinTopkV2 = 48,
        BuiltinSplit = 49,
        BuiltinLogSoftmax = 50,
        BuiltinDelegate = 51,
        BuiltinBidirectionalSequenceLstm = 52,
        BuiltinCast = 53,
        BuiltinPrelu = 54,
        BuiltinMaximum = 55,
        BuiltinArgMax = 56,
        BuiltinMinimum = 57,
        BuiltinLess = 58,
        BuiltinNeg = 59,
        BuiltinPadv2 = 60,
        BuiltinGreater = 61,
        BuiltinGreaterEqual = 62,
        BuiltinLessEqual = 63,
        BuiltinSelect = 64,
        BuiltinSlice = 65,
        BuiltinSin = 66,
        BuiltinTransposeConv = 67,
        BuiltinSparseToDense = 68,
        BuiltinTile = 69,
        BuiltinExpandDims = 70,
        BuiltinEqual = 71,
        BuiltinNotEqual = 72,
        BuiltinLog = 73,
        BuiltinSum = 74,
        BuiltinSqrt = 75,
        BuiltinRsqrt = 76,
        BuiltinShape = 77,
        BuiltinPow = 78,
        BuiltinArgMin = 79,
        BuiltinFakeQuant = 80,
        BuiltinReduceProd = 81,
        BuiltinReduceMax = 82,
        BuiltinPack = 83,
        BuiltinLogicalOr = 84,
        BuiltinOneHot = 85,
        BuiltinLogicalAnd = 86,
        BuiltinLogicalNot = 87,
        BuiltinUnpack = 88,
        BuiltinReduceMin = 89,
        BuiltinFloorDiv = 90,
        BuiltinReduceAny = 91,
        BuiltinSquare = 92,
        BuiltinZerosLike = 93,
        BuiltinFill = 94,
        BuiltinFloorMod = 95,
        BuiltinRange = 96,
        BuiltinResizeNearestNeighbor = 97,
        BuiltinLeakyRelu = 98,
        BuiltinSquaredDifference = 99,
        BuiltinMirrorPad = 100,
        BuiltinAbs = 101,
        BuiltinSplitV = 102,
        BuiltinUnique = 103,
        BuiltinCeil = 104,
        BuiltinReverseV2 = 105,
        BuiltinAddN = 106,
        BuiltinGatherNd = 107,
        BuiltinCos = 108,
        BuiltinWhere = 109,
        BuiltinRank = 110,
        BuiltinElu = 111,
        BuiltinReverseSequence = 112,
        BuiltinMatrixDiag = 113,
        BuiltinQuantize = 114,
        BuiltinMatrixSetDiag = 115,
        BuiltinRound = 116,
        BuiltinHardSwish = 117,
        BuiltinIf = 118,
        BuiltinWhile = 119,
        BuiltinNonMaxSuppressionV4 = 120,
        BuiltinNonMaxSuppressionV5 = 121,
        BuiltinScatterNd = 122,
        BuiltinSelectV2 = 123,
        BuiltinDensify = 124,
        BuiltinSegmentSum = 125,
        BuiltinBatchMatmul = 126,
        BuiltinPlaceholderForGreaterOpCodes = 127,
        BuiltinCumsum = 128,
        BuiltinCallOnce = 129,
        BuiltinBroadcastTo = 130,
        BuiltinRfft2d = 131,
        BuiltinConv3d = 132,
        BuiltinImag = 133,
        BuiltinReal = 134,
        BuiltinComplexAbs = 135,
        BuiltinHashtable = 136,
        BuiltinHashtableFind = 137,
        BuiltinHashtableImport = 138,
        BuiltinHashtableSize = 139,
        BuiltinReduceAll = 140,
        BuiltinConv3dTranspose = 141,
        BuiltinVarHandle = 142,
        BuiltinReadVariable = 143,
        BuiltinAssignVariable = 144,
        BuiltinBroadcastArgs = 145,
        BuiltinRandomStandardNormal = 146,
        BuiltinBucketize = 147,
        BuiltinRandomUniform = 148,
        BuiltinMultinomial = 149,
        BuiltinGelu = 150,
        BuiltinDynamicUpdateSlice = 151,
        BuiltinRelu0To1 = 152,
        BuiltinUnsortedSegmentProd = 153,
        BuiltinUnsortedSegmentMax = 154,
        BuiltinUnsortedSegmentSum = 155,
        BuiltinAtan2 = 156,
        BuiltinUnsortedSegmentMin = 157,
        BuiltinSign = 158,
    }
}
