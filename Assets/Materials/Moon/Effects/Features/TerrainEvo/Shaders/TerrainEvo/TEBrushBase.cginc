#ifndef FILE_TEBRUSHBASE_CGINC
#define FILE_TEBRUSHBASE_CGINC

// Alternative experimental 32 bit control mask layout
//
//	bits  0 - 3	:	Default ID
//	bits  4 - 7	:	Base ID
//	bits  8 - 11:	Overlay ID
//	bits 12 - 15:	Base Weight
//	bits 16 - 19:	Overlay Weight
//	bits 20 - 23:	(unused. Vegetation scale)
//	bits 24 - 31:	Puddle depth
//

//static const uint MASK_DEFAULT_IN			= 0x00000FFF;

static const uint MASK_DEFAULT_MAX			= 0x0000000F;
static const uint MASK_DEFAULT_IN			= 0x0000000F;
static const uint MASK_DEFAULT_OUT			= 0xFFFFFFF0;
static const uint MASK_DEFAULT_SHIFT		= 0;

static const uint MASK_BASE_MAX				= 0x0000000F;
static const uint MASK_BASE_IN				= 0x000000F0;
static const uint MASK_BASE_OUT				= 0xFFFFFF0F;
static const uint MASK_BASE_SHIFT			= 4;

static const uint MASK_OVERLAY_MAX			= 0x0000000F;
static const uint MASK_OVERLAY_IN			= 0x00000F00;
static const uint MASK_OVERLAY_OUT			= 0xFFFFF0FF;
static const uint MASK_OVERLAY_SHIFT		= 8;

static const uint MASK_BASE_WEIGHT_MAX		= 0x0000000F;
static const uint MASK_BASE_WEIGHT_IN		= 0x0000F000;
static const uint MASK_BASE_WEIGHT_OUT		= 0xFFFF0FFF;
static const uint MASK_BASE_WEIGHT_SHIFT	= 12;

static const uint MASK_OVERLAY_WEIGHT_MAX	= 0x0000000F;
static const uint MASK_OVERLAY_WEIGHT_IN	= 0x000F0000;
static const uint MASK_OVERLAY_WEIGHT_OUT	= 0xFFF0FFFF;
static const uint MASK_OVERLAY_WEIGHT_SHIFT	= 16;

static const uint MASK_FOLIAGE_MAX			= 0x0000000F;
static const uint MASK_FOLIAGE_IN			= 0x00F00000;
static const uint MASK_FOLIAGE_OUT			= 0xFF0FFFFF;
static const uint MASK_FOLIAGE_SHIFT		= 20;

static const uint MASK_PUDDLE_MAX			= 0x000000FF;
static const uint MASK_PUDDLE_IN			= 0xFF000000;
static const uint MASK_PUDDLE_OUT			= 0x00FFFFFF;
static const uint MASK_PUDDLE_SHIFT			= 24;

#endif//FILE_TEBRUSHBASE_CGINC