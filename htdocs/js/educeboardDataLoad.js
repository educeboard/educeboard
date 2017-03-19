/********************
pullDown
********************/

;(function($){
	var namespace = 'educeboardDataLoad';

	var $body = $('body');

	/********************
	methods
	********************/

	var methods = {

		/********************
		initialize
		********************/
		initialize:function(method){
			return this.each(function(){

				// オプションをセット
				$(this).data(namespace, $.extend(true, {
					// courseURL : '../../loginCourseList.php'
					// ,userURL : '../../loginCourseMemberList.php'
					// ,authURL : '../../loginAuth.php'
					// ,simulationURL : '../../showSessionList.php'
					// ,commentURL : '../../showComments.php'
					courseURL : 'http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/loginCourseList.php'
					,userURL : 'http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/loginCourseMemberList.php'
					,authURL : 'http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/loginAuth.php'
					,simulationURL : 'http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/showSessionList.php'
					,commentURL : 'http://pb.fm.senshu-u.ac.jp/~tmochi/educeboard/showComments.php'

					,courseButtonText : null
					,userButtonText : null

					,basicInfoObj : {
						cid : null
						,uid : null
						,uname : null
						,simid : null
						,sid : null
						,tid : null
					}

					,courseParameter : {
						xml : 1
					}
					,userParameter : {
						xml : 1
						,cid : null
					}
					,authParameter : {
						xml : 1
						,uid : null
						,pwd : null
					}
					,commentParameter : {
						xml : 1
						,session_id : null
						,tid : null
					}

					,$simulationList : $('[data-' + namespace + '-parts="simulationList"]')

					,courseButtonID : 'coursePulldownButton'
					,userButtonID : 'namePulldownButton'
					,pwdButtonID : 'loginButton'

					,pwdObjClass : 'elPassword'

					,simulationListClass : 'elSimulationList'
					,sessionListClass : 'elSessionList'
					,trialListClass : 'elTrialList'

					,commentClass : 'elComment'
					,commentUserClass : 'elUser'
					,commentTimeClass : 'elTimeStamp'

					,coursePulldownID : null
					,userPulldownID : null

					,$coursePulldownBtn:null
					,$userPulldownBtn:null

					,$coursePulldownObj : null
					,$userPulldownObj : null

					,$pwdInputObj : null

					,$loginButtonObj : null

					,$commentArea : $('[data-' + namespace + '-parts="commentArea"]')

					,$loginInfo : $('[data-' + namespace + '-parts="loginInfo"]')
					,$username : $('[data-' + namespace + '-parts="username"]')

					// takeIDの次のmethodの切替用
					,nextMethod : 'course'

					// callback
					,courseLoaded : null
					,userLoaded : null
					,beforeAuth : null
					,authSuccess : null
					,authFail : null
					,userLogout: null
					,simulationLoaded : null
					,commentLoaded : null
					,idResult : null

				}, method));

				// dataコピー
				var $this = $(this)
				,options = $this.data(namespace);

				options.$coursePulldownBtn = $this.find('#' + options.courseButtonID);
				options.courseButtonText = options.$coursePulldownBtn.text();
				options.coursePulldownID = options.$coursePulldownBtn.attr('aria-controles');
				options.$coursePulldownObj = $this.find('#' + options.coursePulldownID);

				console.log(options.$coursePulldownObj);

				options.$userPulldownBtn = $this.find('#' + options.userButtonID);
				options.userButtonText = options.$userPulldownBtn.text();
				options.userPulldownID = options.$userPulldownBtn.attr('aria-controles');
				options.$userPulldownObj = $this.find('#' + options.userPulldownID);

				options.$pwdInputObj = $this.find('.' + options.pwdObjClass).find('input[type="password"]');

				options.$loginButton = $this.find('#' + options.pwdButtonID);


				methods.courseLoader.apply($this);
				methods.userLogout.apply($this);

			});
		}

		/********************
		courseLoader
		********************/
		,courseLoader:function(){
			var $this = $(this)
			,options = $this.data(namespace);



			// 選択し直すときに初期化する処理
			options.$coursePulldownBtn.on('click.' + namespace, function(){

				// userボタンのテキストを戻し、非アクティブにする
				options.$userPulldownBtn
				.text(options.userButtonText)
				.parents('[aria-activedescendant]')
				.attr('aria-activedescendant','');

				// プルダウンの中身を全部消す
				options.$userPulldownObj.find('li').remove();

				// プルダウンを閉じる
				options.$userPulldownObj.attr('aria-expanded',false);

				// パスワード入力部分を入力できないようにする
				options.$pwdInputObj.attr('disabled','disabled').val('');

				// ログインボタンを非アクティブに
				options.$loginButton.parents('[aria-activedescendant]').attr('aria-activedescendant','');

				// 次のロード
				options.nextMethod = 'userLoader';
			});

			// ajax開始
			$.ajax({
				type : 'GET'
				,url : options.courseURL
				,data : options.courseParameter
			})
			// 成功時の処理
			.done(function(xml){
				// cidとcnameを格納するオブジェクトを作成
				var courseData = {
					cid : []
					,cname : []
				};
				// xmlデータからcidとcnameを探し格納
				$(xml).find('CourseList').each(function(i){
					courseData['cid'][i] = $(this).find('cid').text();
					courseData['cname'][i] = $(this).find('cname').text();
				});

				// for文準備
				var courseArrayLength = courseData['cid'].length
				,liArray = []
				,i;
				// ulに入るliを作成。cidをdata属性で持っておく
				for(i = 0; i < courseArrayLength; i++){
					liArray[i] = $('<li />').attr('data-cid' ,courseData['cid'][i]);
				}
				// liにaタグを格納
				for(i = 0; i < courseArrayLength; i++){
					var aObj = $('<a href="#" />').text(courseData['cname'][i]);
					liArray[i].append(aObj);
				}
				// プルダウンにliをappend
				for(i = 0; i < courseArrayLength; i++){
					options.$coursePulldownObj.append(liArray[i]);
				}

				// ボタンをactiveにするために属性を付与
				options.$coursePulldownBtn.parents('[aria-activedescendant]').attr('aria-activedescendant',options.courseButtonID);

				// liデータも引数としてtakeIDを実行
				options.nextMethod = 'userLoader';
				methods.takeID.apply([$this , liArray]);

				// callback実行
				methods.applyCallback.apply([$this, 'courseLoaded']);

			})
			.fail(function(){
				alert('コースリストの取得に失敗しました（・∀・）');
			});

		}

		/********************
		userLoader
		********************/
		,userLoader:function(){
			var $this = $(this)
			,options = $this.data(namespace);

			// 選択し直すときに初期化する処理
			options.$userPulldownBtn.on('click.' + namespace, function(){


				// パスワード入力部分を入力できないようにする
				options.$pwdInputObj.attr('disabled','disabled').val('');

				// ログインボタンを非アクティブに
				options.$loginButton.parents('[aria-activedescendant]').attr('aria-activedescendant','');

				// 次のロード
				options.nextMethod = 'authLogin';
			});

			options.userParameter.cid = options.basicInfoObj.cid;

			// ajax開始
			$.ajax({
				type : 'GET'
				,url : options.userURL
				,data : options.userParameter
			})
			.done(function(xml){
				// uidとunameを格納するオブジェクトを作成
				var userData = {
					uid : []
					,uname : []
				};
				// xmlデータからcidとcnameを探し格納
				$(xml).find('UserList').each(function(i){
					userData['uid'][i] = $(this).find('uid').text();
					userData['uname'][i] = $(this).find('username').text();
				});

				// for文準備
				var userArrayLength = userData['uid'].length
				,liArray = []
				,i;
				// ulに入るliを作成。uidをdata属性で持っておく
				for(i = 0; i < userArrayLength; i++){
					liArray[i] = $('<li />').attr('data-uid' ,userData['uid'][i]).attr('data-uname',userData['uname'][i]);
				}
				// liにaタグを格納
				for(i = 0; i < userArrayLength; i++){
					var aObj = $('<a href="#" />').text(userData['uname'][i]);
					liArray[i].append(aObj);
				}
				// プルダウンにliをappend
				for(i = 0; i < userArrayLength; i++){
					options.$userPulldownObj.append(liArray[i]);
				}

				// ボタンをactiveにするために属性を付与
				options.$userPulldownBtn.parents('[aria-activedescendant]').attr('aria-activedescendant',options.userButtonID);

				// liデータも引数としてtakeIDを実行
				options.nextMethod = 'authLogin';
				methods.takeID.apply([$this , liArray]);

				// callback実行
				methods.applyCallback.apply([$this, 'userLoaded']);
			})
			.fail(function(){
				alert('ユーザリストの取得に失敗しました（・∀・）');
			});
		}

		/********************
		authLogin
		********************/
		,authLogin:function(){
			var $this = $(this)
			,options = $this.data(namespace);

			methods.applyCallback.apply([$this,'beforeAuth'])

			options.authParameter.uid = options.basicInfoObj.uid;

			options.$pwdInputObj.removeAttr('disabled').select();

			options.$loginButton.parents('[aria-activedescendant]').attr('aria-activedescendant',options.pwdButtonID);

			options.$loginButton.off('.' + namespace);
			options.$pwdInputObj.off('.' + namespace);

			options.$loginButton.on('click.' + namespace, loginHandler);
			options.$pwdInputObj.on('keypress.' + namespace, function(e){
				if(e.which == 13){
					loginHandler(e);
				}
			});



			function loginHandler(e){
				e.preventDefault();

				options.authParameter.pwd = options.$pwdInputObj.val();

				// ajax開始
				$.ajax({
					type : 'GET'
					,url : options.authURL
					,data : options.authParameter
				})
				.done(function(data){
					var loginFlag = $(data).find('Auth').text();
					if(loginFlag == "false"){


						options.$loginInfo.attr('aria-hidden',false);
						options.$username.text(options.basicInfoObj.uname);

						$body.data('educeboardBasicInfo',{
							uid:options.basicInfoObj.uid
							,uname:options.basicInfoObj.uname
						});


						methods.simulationLoader.apply($this);

						methods.applyCallback.apply([$this,'authSuccess']);

					}
					else{
						methods.applyCallback.apply([$this,'authFail']);

						alert('パスワードが違うのかもしれません！');

						options.$pwdInputObj.select();

						return false;
					}
				})
				.fail(function(){
					alert('ログイン情報の取得に失敗しました（・∀・）');
				});
			}

		}

		/********************
		userLogout
		********************/
		,userLogout:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			,$loginInfo = options.$loginInfo
			,$username = options.$username;

			$this.on('click.' + namespace, '[data-' + namespace + '-parts="logout"]', function(e){
				var $target = $(this);

				$loginInfo.attr('aria-hidden','true');
				$username.text('');

				methods.applyCallback.apply([$this,'userLogout']);
			})

		}

		/********************
		simulationLoader
		********************/
		,simulationLoader:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			,parameter = {
				cid : options.basicInfoObj.cid
				,uid : options.basicInfoObj.uid
			};
			console.log(parameter);

			$.ajax({
				type : 'GET'
				,url : options.simulationURL
				,data : parameter
			})
			.done(function(xml){

				$listDom = options.$simulationList;
				$listDom.empty();

				console.log($listDom);

				options.$pwdInputObj.val('');

				var $simulation;

				// simulation
				$(xml).find('simulation').each(function(i){

					// eachのthisを格納
					$simulation = $(this)

					// simulationのidとnameを取得
					,simId = $simulation.find('sim_id').text()
					,simName = $simulation.find('sim_name').text()

					// liタグを定義してdlタグを入れる
					,$simLi = $('<li />').attr('data-listExpand-level',0)
					.append($('<dl />'))

					// sessionのexpandが入るddタグを生成し、ulタグも格納
					,$sessionExpandWrap = $('<dd aria-expanded="false" id="expand' + i + '" />')
					.append($('<ul />',{
						class : options.sessionListClass
					}));

					// dlタグにdtタグと上で作成したddタグを入れる
					$simLi.find('dl')
					.append($('<dt><a href="#" role="button" aria-controles="expand' + i + '">' + simName + '</a></dt>'))
					.append($sessionExpandWrap);


					// session
					$simulation.find('session').each(function(j){

						// eachのthisを格納
						var $session = $(this)

						// sessionのidとnameを格納
						,sessionId = $session.find('session_id').text()
						,sessionName = $session.find('session_name').text()

						// simulationのiとsessionのjでexpandの番号を作る
						,expandNum = i + '-' + j

						// liを作成しdlを入れる 階層は1
						,$sessionLi = $('<li />').attr('data-listExpand-level',1)
						.append($('<dl />'))

						// trialのexpandが入るdd作成し、ulタグも格納
						,$trialExpandWrap = $('<dd aria-expanded="false" id="expand' + expandNum + '" />')
						.append($('<ul />',{
							class : options.trialListClass
						}));

						// dlタグにdtタグと上で作成したddタグを入れる
						$sessionLi.find('dl')
						.append($('<dt><a href="#" role="button" aria-controles="expand' + expandNum + '">' + sessionName + '</a></dt>'))
						.append($trialExpandWrap);

						// ddタグの中のulタグに作成したliタグを「sessionExpand」に格納
						$sessionExpandWrap.find('.' + options.sessionListClass).append($sessionLi);


						// trial
						$session.find('trial').each(function(k){

							// eachのthisを格納
							var $trial = $(this)

							,trialId = $trial.find('trial_id').text()
							,timestamp = $trial.find('timestamp').text()

							,$trialLi = $('<li><p><a href="#" data-unitySyncData-parts="sendID" /></p></li>')

							,$a = $trialLi.find('a');


							$a.data('id',{
								simulationId : simId
								,sessionId : sessionId
								,trialId : trialId
							})
							.text(timestamp)
							.on('click.' + namespace, function(e){
								e.preventDefault();

								var resultID = $(e.target).data().id;

								options.basicInfoObj.sid = parseInt(resultID.sessionId);
								options.basicInfoObj.tid = parseInt(resultID.trialId);

								// educeboardプレイヤー表示
								$('#educeboard').attr('aria-hidden',false);

								// ログイン画面非表示
								$('#login').attr('aria-hidden',true);

								// commentLoader実行
								methods.commentLoader.apply($this);

								methods.applyCallback.apply([$this , 'idResult']);
							});

							$trialExpandWrap.find('.' + options.trialListClass).append($trialLi);

						});

					});


					$listDom.append($simLi);

				});

				methods.applyCallback.apply([$this , 'simulationLoaded']);
			});
		}

		/********************
		commentLoader
		********************/
		,commentLoader:function(){
			var $this = $(this)
			,options = $this.data(namespace)
			,$commentArea = options.$commentArea;

			options.commentParameter.session_id = options.basicInfoObj.sid;
			options.commentParameter.tid = options.basicInfoObj.tid;

			// ajax開始
			$.ajax({
				type : 'GET'
				,url : options.commentURL
				,data : options.commentParameter
			})
			// 成功時の処理
			.done(function(xml){
				console.log(xml);

				var $comment
				,commentText
				,userText
				,timeText
				,timeObject = {
					hour:0
					,minute:0
					,second:0
					,raw:''
				}
				,i
				,$dl
				,$li;

				$(xml).find('Comments').each(function(i){
					$comment = $(this);

					$li = $('<li />');
					$dl = $('<dl />');

					commentText = $comment.find('text').text();
					userText = $comment.find('username').text();
					timeObject.raw = parseInt($comment.find('time').text());

					timeObject.second = timeObject.raw;
					timeObject.minute = parseInt(timeObject.second / 60);
					timeObject.hour = parseInt(timeObject.minute / 60);

					timeObject.minute = timeObject.minute - 60 * timeObject.hour;
					timeObject.second = timeObject.second - 60 * timeObject.minute;

					timeText = '';
					for(i in timeObject){

						if(i == 'raw'){
							continue;
						}

						if(timeObject[i] < 10){
							timeObject[i] = '0' + timeObject[i];
						}

						timeText += timeObject[i];

						if(i != 'second'){
							timeText += ':';
						}
					}

					$('<dt />').text(commentText).toggleClass(options.commentClass,true).appendTo($dl);
					$('<dd />').text(userText).toggleClass(options.commentUserClass,true).appendTo($dl);
					$('<dd />').text(timeText).toggleClass(options.commentTimeClass,true).appendTo($dl);

					$li.attr('data-commentSyncScroll-timestamp',timeObject.raw).append($dl);
					$commentArea.append($li);

				});

				methods.applyCallback.apply([$this,'commentLoaded']);
			})
			.fail(function(){
				alert('コメントの取得に失敗しました（・∀・）');
			});
		}

		/********************
		takeID
		********************/
		,takeID:function(){
			var $this = $(this)[0]
			,options = $this.data(namespace)
			// liデータを取得
			,array = $(this)[1]
			// for文準備
			,i
			,length = array.length;
			for(i = 0; i < length; i++){
				array[i].on('click.' + namespace , function(e){
					e.preventDefault();

					// クリックされたもの
					var $target = $(this)
					,idData = $target.data();

					// unameの処理

					// IDを上書き
					$.extend(options.basicInfoObj , idData);

					methods[options.nextMethod].apply($this);
				})
			}
		}

		/********************
		applyCallback
		********************/
		,applyCallback:function(){
			var $this = $(this)[0]
			,options = $this.data(namespace)
			,callback = $(this)[1];


			if(typeof options[callback] === 'function'){;
				options[callback]();
			}
		}

		/********************
		destroy
		********************/
		,destroy:function(){


		}
	};

	/********************
	全プラグイン共通
	********************/
	$.fn[namespace] = function(method){
		if(methods[method]){
			return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
		}
		else if(typeof method === 'object' || !method){
			return methods.initialize.apply(this, arguments);
		}
	}

})(jQuery);
