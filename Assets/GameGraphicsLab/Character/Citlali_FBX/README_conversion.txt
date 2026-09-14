Citlali glTF -> FBX conversion package
=====================================

Recommended file:
- Citlali_rigged_materials_textures.fbx
  Includes mesh, UV0, vertex normals, 22 material assignments, relative Base Color texture links, skeleton hierarchy, and skin weights.

Fallback file:
- Citlali_materials_textures.fbx
  Static-mesh fallback preserving mesh/material/texture links but without skin/bones.

Textures:
- textures/material_baseColor.png
- textures/material_0_baseColor.png
- textures/material_4_baseColor.png
- textures/material_8_baseColor.png
- textures/material_20_baseColor.png

Unity usage:
1. Copy the whole Citlali_FBX folder into Assets. Keep the textures subfolder beside the FBX files.
2. Select the rigged FBX and check Materials / Extract Materials if Unity does not automatically create external materials.
3. Texture references are stored as relative paths under textures/.
4. The source scene's wrapper transform is baked into the mesh/skeleton scale.

Material / texture mapping:
00 material -> textures/material_baseColor.png
01 material_0 -> textures/material_0_baseColor.png
02 material_1 -> textures/material_0_baseColor.png
03 material_2 -> textures/material_0_baseColor.png
04 material_3 -> (none)
05 material_4 -> textures/material_4_baseColor.png
06 material_5 -> textures/material_0_baseColor.png
07 material_6 -> (none)
08 material_7 -> textures/material_0_baseColor.png
09 material_8 -> textures/material_8_baseColor.png
10 material_9 -> textures/material_8_baseColor.png
11 material_10 -> textures/material_4_baseColor.png
12 material_11 -> textures/material_4_baseColor.png
13 material_12 -> textures/material_4_baseColor.png
14 material_13 -> textures/material_8_baseColor.png
15 material_14 -> textures/material_4_baseColor.png
16 material_15 -> textures/material_4_baseColor.png
17 material_16 -> textures/material_4_baseColor.png
18 material_17 -> textures/material_4_baseColor.png
19 material_18 -> textures/material_8_baseColor.png
20 material_19 -> textures/material_0_baseColor.png
21 material_20 -> textures/material_20_baseColor.png

Original source license is included in license.txt (CC-BY-4.0).
