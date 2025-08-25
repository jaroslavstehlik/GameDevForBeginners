# installation

## Before we start

- [Instal Unity3D minimum version: 6000.0.41f1 from Unity hub.](https://unity.com/download)
- [Install Git (minimum version 2.14.0) on your computer.](https://git-scm.com/downloads)
- [Install Git LFS on your computer.](https://git-lfs.com/)

## Create new Unity project

- Select Unity version 6000.0.41f1
- Select Universal 3D preset
- Name your project
- Create project  
<img src="img/create_project.png" alt="create unity project"/>

## Import package

- Open Package Manager in Unity3D from Window/Package Manager.
- Open the add <img src="img/upm_icon_add.png" alt="git url"/> menu in the Package Manager’s toolbar.
- The options for adding packages appear.  
<img src="img/upm_ui_giturl.png" alt="git url"/>

- Select Add package from git URL from the add menu.  
- Fill the text box with URL:  
`https://github.com/jaroslavstehlik/GameDevForBeginners.git`
- Hit OK.  

## Import samples

- Open menu Window/Package Manager/Packages
- Select Game dev for beginners package.
- Select samples tab
- Import GameDev Core  
<img src="img/import_samples.png" alt="import samples"/>

## Prepare project

### Text mesh pro essential resources
- Open menu Windows/TextMeshPro/Import TMP Essential resources  
<img src="img/tmp_essentials.png" alt="import text mesh pro essentials"/>

### Rename layers
- Open menu Edit/Project Settings/Tags and Layers  
<img src="img/tags_layers.png" alt="open tags and layers"/>
- Open layer presets  
<img src="img/layers_preset_icon.png" alt="open preset"/>
- Apply layer preset  
<img src="img/layers_preset_apply.png" alt="apply preset"/>

### Setup physics collision matrix
- Open menu Edit/Project Settings/Physics/Settings  
<img src="img/project_settings_physics.png" alt="open physics settings"/>
- Enable layers as in picture  
<img src="img/physics_mask.png" alt="change collision matrix"/>
- Repeat for 2D physics


You are now all set!