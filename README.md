動作は
/users/notes
で取得したノートを
/ap/show
で照会してます

## 使い方
config.iniをいい感じに変えます
- App
  - DelayはLocalで指定したサーバーへのリクエスト間隔(ミリ秒)
- Remote
  - Hostはリモートのドメイン
  - UserCountは全件取得するユーザー数
  - Remote_User_nのIdはリモートのIDをコピペ
  - UserCountの分Remote_User_nを増やせます
    - [Remote_User_5] id = ""
- Local
    - Host、一括照会してローカルに表示させたいドメイン
    - APIトークン(権限全無効でも使えます)

全投稿(Renote含め)を順次ローカルで同期的に照会するので時間がかかります  
  
リモートへのAPIリクエスト間隔はDelay*100です(照会にかかる時間も含めるともう少しかかります)  
  
レートリミットがかかる間隔が分からないので自鯖ならレートリミット0にすると楽です。
