# 一种更加完备的集会系统

An Augmented Assembly System

## 简介

**一种更加完备的集会系统** 由 **VRCD 开发者系列活动运营组** 整合制作，用于为大/中/小型社团活动。

## 使用方式

### 1、事前准备

如果您的世界中含有以下插件，请先删除以下插件。

- [Sliden](https://sliden.app/) —— [ちくわ物産](https://chikuwa-products.booth.pm/)
- [Real-time Clocks](https://github.com/VirtualVisions/VowganPrefabs/releases/tag/Clocks) ——[Vowgan](https://github.com/Vowgan)

您需要在世界中提前装载以下核心插件：

- [Pro TV](https://protv.dev) —— [ArchiTechVR](hhttps://gitlab.com/techanon)
    > 提供视频/音频/直播功能
- [Udon Voice Utilities](https://github.com/Guribo/UdonVoiceUtils) —— [Guribo](https://github.com/Guribo)
    > 提供音频分区与广播功能
- [VRChat 技术导播系统]() —— [Ark_Magellan]()
    > 提供导播与画面导出功能

### 2、安装插件

将下载并解压的 .unitypackage 文件拖入您的 Unity 项目。

---

## 组件说明

### AAAS-Main

#### 激光笔（Laser Pointer）

> AAAS-Main > Laser Pointer

激光笔（Laser Pointer）是一个来自 [UdonToolKit](https://github.com/orels1/UdonToolkit) 的插件，你可以在演讲中使用激光笔指向屏幕并留下明显的“光斑”，用于引导观众视线和强调画面重点。

- 激光笔会和 Default 图层下的碰撞体网格互动，如果你不希望激光点落在碰撞体检测面上，你需要将相应的碰撞体放入 Default 层之外的任何层级。

- 如果你想要修改激光笔能够互动的图层，你可以查看 AAAS-Main/Laser Pointer 对象的 Laser Pointer 组件，并将 Hit Layers 属性修改为你想要的图层。

---

#### 操作面板

> AAAS-Main > PlayerSystem > SpeakerDock

操作面板是主持人和演讲者互动并操作的核心面板，你可以使用这个面板：

- 播放视频/PPT
- 调整视频/PPT进度
- 重置麦克风/激光笔
- 显示/隐藏演讲者工具

---

### AAAS-TLP

AAAS 的音频组件合集。

---

#### 麦克风

> AAAS-TLP > TLP_Microphone

使用麦克风的玩家声音将被其他玩家无视距离以最大音量接收。

使用方式：拾取麦克风。

> 如果您希望操作面板中的麦克风传送按钮工作，您需要提前将麦克风绑定至 AAAS-Main > PlayerSystem > SpeakerDock > SpeakerTablet > UI > GameObject > SpeakerSLock (1) 的 Teleport Game Object 脚本中。（将麦克风游戏对象拖拽至 Target Game Object 上）

---

#### 扩音区

> AAAS-TLP > TLP_AmplificationZone

在此碰撞体内的玩家声音将被其他玩家无视距离以最大音量接收。

布置方式：将此区域的碰撞体覆盖至你希望扩音效果发生的地方。

---

#### 静音区

> AAAS-TLP > TLP_QuietZone

在此碰撞体内的玩家将只能听见处于同一个区域的玩家的声音。（扩音区声音会被屏蔽）

布置方式：将此区域的碰撞体覆盖至你希望静音效果发生的地方。

---

## 开发

### 1. 为 U# 配置 Git（适用于 Windows）

U# 编译器会对 scene 、 prefab 和 scene 文件进行大量 GUID 变更用于创建编译成果的关联。这会导致版本控制中的变更追踪变得异常艰难。因此，我们要求在提交代码修改时不包括这些易变的变更。使用以下 Git 配置指令，配合任意运行时，允许 Git 版本控制自动忽略这些变更，以防止意外的变更提交。

#### 选项 1)：使用 Python 运行时

```sh
git config filter.usharp-prefab.process "python .gitscripts/filter_usharp_process.py"
```

#### 选项 2)：使用 Node.js 运行时

```sh
git config filter.usharp-prefab.clean "node .gitscripts/filter-usharp-prefab.js"
```

#### 重新规范化仓库

**<span style="color:red">⚠注意：每当清理过滤器更改时，存储库应该重新规范化。参考：[Git - attributes 文档](https://git-scm.com/docs/gitattributes#:~:text=Note:%20Whenever%20the%20clean%20filter%20is%20changed,%20the%20repo%20should%20be%20renormalized)</span>**

```sh
git add --renormalize .
```

### 2. 为 Unity 配置 Git （可选）

你可以选择使用 Unity 的 YAML 合并工具来处理任何潜在的 Git 合并冲突（参见：[Unity 文档](https://docs.unity3d.com/2022.3/Documentation/Manual/SmartMerge.html)）。其允许 Git:

>以语义正确的方式合并 scene 、 prefab 和 scene 文件

**<span style="color:red">⚠注意：该工具仅保证 YAML 语义上的正确性，你仍需确保合并成果内容的正确性。</span>**

如果你理解并需要这项功能，如下 Git 配置指令可以被用于进行该功能的配置：

```sh
git config merge.unityyamlmerge.driver '"C:/Program Files/Unity/Hub/Editor/2022.3.22f1/Editor/Data/Tools/UnityYAMLMerge.exe" merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"'
```

其中，根据 Unity 安装方式的不同，指令中的 `C:/Program Files/Unity/Hub/Editor/2022.3.22f1/Editor/Data/Tools/UnityYAMLMerge.exe` 可能需要被对应的修改。

## 再配布相关

集会功能插件由多个插件功能整合而成。

[Real-time Clocks](https://github.com/VirtualVisions/VowganPrefabs/releases/tag/Clocks) ——[Vowgan](https://github.com/Vowgan)

> [MIT License](https://opensource.org/license/mit/)

[Sliden](https://sliden.app/) —— [ちくわ物産](https://chikuwa-products.booth.pm/)

> [V3 License](https://booth.pm/downloadables/3702043)

[Pro TV](https://protv.dev) —— [ArchiTechVR](hhttps://gitlab.com/techanon)

> [ISC License](https://www.isc.org/licenses/)

[UdonUI](https://github.com/Asitir/UdonUI) —— [Asitir](https://github.com/Asitir)

> [MIT License](https://opensource.org/license/mit/)

[UdonToolKit](https://github.com/orels1/UdonToolkit) —— [orels1](https://github.com/orels1)

> [MIT License](https://opensource.org/license/mit/)

## 许可证

**许可证名称**：ISC + 内容友好许可（自定义条款）

> 基于 ISC 许可证，附加以下条款

### 使用权限

允许个人或法人以营利或非营利目的使用本软件 / 内容（包括但不限于嵌入产品、二次创作等）。

允许在社交平台、在线游戏平台（如 VRChat）上传和公开使用。

### 内容限制

允许：性相关、暴力性表达，但需明确分类标注（如 “成人内容”“暴力分级”）。

禁止：政治活动、宗教活动相关表达（私人范围内使用除外）。

### 修改与加工

允许调整、修改、衍生创作，可委托第三方进行加工。

修改后需保留原作品的版权声明（除非获得原作者书面豁免）。

### 再发布与发布

未修改版本：可免费再发布（如分享链接、分发文件）。

修改版本：仅允许免费再发布，禁止收费或商业分发（除非单独获得原作者授权）。

### 媒体与产品使用

允许在影像、出版物、周边商品及软件嵌入中使用本内容。

允许将本内容作为素材进行二次创作（如服装数据），发布时无需额外授权。

### 其他条款

无需信用标记：使用或再发布时可不标注原作者或版权信息。

禁止权利转让：不得将本许可证下的权利义务整体或部分转让给第三方。

免责声明：本内容按 “原样” 提供，作者不对使用后果承担任何责任。

## 插件作者

- 可可脂
- WangQAQ
- Ark_Magellan
- Asitir
- SKPlol
