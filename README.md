# Echeneis

[ORCA GC Controller](https://github.com/yatsuna827/Orca-GC-Controller)と連携して、マクロ実行後の条件判定とループを行うプログラム

## Usage

`Echeneis.exe`を実行して、指示に従ってください。

```
.\Echeneis
```

## Requirements

### [ORCA GC Controller](https://github.com/yatsuna827/Orca-GC-Controller)

OrcaWrapperフォルダ直下にORCA GC Controllerフォルダを作り、`ORCA GC Controller.exe`を配置してビルドします。

## Sample

ICriteriaを実装したDLLで条件判定...みたいなことを考えていますが飽きたのでそこまでしていません。

実装サンプルであるCriteriaImplは、「つよさをみる」画面の実数値が`criteriaimpl.config.json`と一致するまでループします。
