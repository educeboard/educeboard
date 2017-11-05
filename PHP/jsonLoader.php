<?php
#Flash Ver.用
header('Access-Control-Allow-Origin: *');
error_reporting(E_ALL ^ E_DEPRECATED);
if(isset($_GET['session_id'])){
	$session_id = $_GET['session_id'];
}
if(isset($_GET['tid'])){
	$tid = $_GET['tid'];
}
if(isset($_GET['json'])){
	$json = $_GET['json'];
}
elseif(!isset($_GET['json'])){
	$json = 0;
}
if(isset($_GET['xml'])){
	$xml = $_GET['xml'];
}
if(isset($_GET['pack_id'])){
	$pack_id = $_GET['pack_id'];
}

mb_language("uni");
mb_internal_encoding("utf8");
mb_http_input("auto");
mb_http_output("utf8");
$url = "localhost";

$user = "root";
$pass = "hoge1234";
$db = "educeboard";
$str = "";
$link = mysql_connect($url,$user,$pass) or die("MySQLへの接続に失敗しました。");

$sdb = mysql_select_db($db,$link) or die("データベースの選択に失敗しました。");

$pack_num = 10000;
//	    $sql = sprintf("INSERT into MarkersLocator values ('','$session_id','$tid','$mid','$TS','$x','$y','$z','$color','$direction1','$direction2','$direction3','$status')");
//	    $result = mysql_query($sql, $link) or die("クエリ_CSVの送信に失敗しました。<br />SQL:".$sql);


// Following script is for checking variables in the MarkersLocator Table
// Delete for the final version.

$sql = sprintf("SELECT id,mid,TS,x,y,z,color,direction1,direction2,direction3,status from MarkersLocator where session_id=$session_id and tid=0");

$result = mysql_query($sql, $link) or die("クエリの送信に失敗しました。<br />SQL:".$sql);
//   echo "<?xml version=\"1.0\" encoding=\"UTF-8\"\n";
//   echo "<data>\n";

#	  $x_max = 180;  // 160;
#	  $x_min = -190; // -170;
#	  $y_max = 110; // 130;
#	  $y_min = -110; // -130
	  $x_max = 590;  // 160;
	  $x_min = 30; // -170;
	  $y_max = 439; // 130;
	  $y_min = 38; // -130

	  $x_length = $x_max - $x_min;
	  $y_length = $y_max - $y_min;

	  $sql2 = "";
	  if(isset($_GET['pack_id']))
	  {
	  	$pack = $pack_id*$pack_num;
	  	$sql2 = sprintf("SELECT id,mid,TS,x,y,z,color,direction1,direction2,direction3,status 
	  		from MarkersLocator 
	  		where session_id=$session_id and tid=$tid
	  		limit $pack , $pack_num ");
	  }
	  else
	  {
	  	$sql2 = sprintf("SELECT id,mid,TS,x,y,z,color,direction1,direction2,direction3,status from MarkersLocator where session_id=$session_id and tid=$tid");
	  }
	  $result2 = mysql_query ($sql2, $link) or die("クエリの送信に失敗しました。<br /> SQL:".$sql2);

	  if($xml != 2){
	  	header ("Content-Type: text/xml; charset=UTF-8");
	  	$str = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
	  	$str .= "<data>\n";
	  	while ($row = mysql_fetch_assoc($result2)) {
	  		$x_row = round((( $row['x'] - $x_min ) / $x_length )*100,2);
	  		$y_row = round((( $row['y'] - $y_min ) / $y_length )*100*0.9,2);
#	    $y_row = round((( $row['y'] - $y_min ) / $y_length )*100-10,2);
/*	      print ("<MarkerLocator id='".$row['id']."'>\n");
#	      print ("<item id='".$row['id']."'>\n");
	      echo "<mid>";
	      echo $row['mid'];
	      echo "</mid>\n";
	      echo "<TS>";
#	      echo "<time>";
	      echo $row['TS'];
	      echo "</TS>\n";
#	      echo "</time>\n";
	      echo "<x>";
#	      echo "<xzahyo>";
	      echo $x_row;
	      echo "</x>\n";
#	      echo "</xzahyo>\n";
	      echo "<y>";
#	      echo "<yzahyo>";
	      echo $y_row;
	      echo "</y>\n";
#	      echo "</yzahyo>\n";
	      echo "<z>";
	      echo $row['z'];
	      echo "</z>\n";
	      echo " <color>";
	      echo $row['color'];
	      echo "</color>\n";
#		  include("showActorImage.php?mid=" . $row['mid'] . "&color=" . $row['color']);
	      echo "<direction1>";
	      echo $row['direction1'];
	      echo "</direction1>\n";
	      echo "<direction2>";
	      echo $row['direction2'];
	      echo "</direction2>\n";
	      echo "<direction3>";
	      echo $row['direction3'];
	      echo "</direction3>\n";
	      echo "<status>";
	      echo $row['status'];
	      echo "</status>\n";
	      echo "</MarkerLocator>\n";
#	      echo "</item>\n";*/

	      $str .= "<MarkerLocator id='".$row['id']."'>\n";
#	      print ("<item id='".$row['id']."'>\n");
	      $str .= "<mid>";
	      $str .= $row['mid'];
	      $str .= "</mid>\n";
	      $str .= "<TS>";
#	      echo "<time>";
	      $str .= $row['TS'];
	      $str .= "</TS>\n";
#	      echo "</time>\n";
	      $str .= "<x>";
#	      echo "<xzahyo>";
	      $str .= $x_row;
	      $str .= "</x>\n";
#	      echo "</xzahyo>\n";
	      $str .= "<y>";
#	      echo "<yzahyo>";
	      $str .= $y_row;
	      $str .= "</y>\n";
#	      echo "</yzahyo>\n";
	      $str .= "<z>";
	      $str .= $row['z'];
	      $str .= "</z>\n";
	      $str .= " <color>";
	      $str .= $row['color'];
	      $str .= "</color>\n";
#		  include("showActorImage.php?mid=" . $row['mid'] . "&color=" . $row['color']);
	      $str .= "<direction1>";
#	      if ($row['mid'] == 1009) { 
#	      	if ($row['direction1'] < 0){
#	      		$row['direction1'] = $row['direction1'] + 180;
#	      	}
#	      	else {
#	      		$row['direction1'] = $row['direction1'] -180;
#	      	}
#	      }
#	      if ($row['mid'] == 1010) {
#	      	if ($row['direction1'] < 0){
#	      		$row['direction1'] = $row['direction1'] + 180;
#	      	}
#	      	else {
#	      		$row['direction1'] = $row['direction1'] -180;
#	      	}
#	      }
	      $str .= $row['direction1'];
	      $str .= "</direction1>\n";
	      $str .= "<direction2>";
	      $str .= $row['direction2'];
	      $str .= "</direction2>\n";
	      $str .= "<direction3>";
	      $str .= $row['direction3'];
	      $str .= "</direction3>\n";
	      $str .= "<status>";
	      $str .= $row['status'];
	      $str .= "</status>\n";
	      $str .= "</MarkerLocator>\n";
#	      echo "</item>\n";*/
	  }
	  $str .= "</data>";
	  header("Content-Length: " .strlen($str));
	  echo $str;
	}
	else{
		header ("Content-Type: application/json; charset=UTF-8");
		ob_start();

		if ($json == 1){
			$data = array();
		}
		else {
			$data = array();
		}
		$cnt = 0;
		$index = 1;
		$thinJudge = 0;
		$thinIdJudge = [];
		$isThin = false;
		while ($row = mysql_fetch_assoc($result2)) {
			$decimal = substr($row['TS'], strpos($row['TS'], '.') + 1);
			$decimal = intval(substr($decimal, 0, 1));

			// if ($decimal % 3 !== 0 || $decimal === 0) {
			// 	continue;
			// }

			if ($thinJudge !== $decimal / 3) {
				$thinJudge = $decimal / 3;
				$thinIdJudge = [];
			}

			for ($i = 0; $i < count($thinIdJudge); $i++) {
				if ($thinIdJudge[$i] === $row['mid'] && $thinJudge === $decimal / 3) {
					$isThin = true;
					break;
				}
			}

			// if ($isThin) {
			// 	$isThin = false;
			// 	continue;
			// }
			$thinIdJudge[] = $row['mid'];
			$cnt++;

			$x_row = round((( $row['x'] - $x_min ) / $x_length )*100,2);
			$y_row = round((( $row['y'] - $y_min ) / $y_length )*100*0.9,2);

			if ($row['mid'] == 1009) { 
				if ($row['direction1'] < 0){
					$row['direction1'] = $row['direction1'] + 180;
				}
				else {
					$row['direction1'] = $row['direction1'] -180;
				}
			}
			if ($row['mid'] == 1010) {
				if ($row['direction1'] < 0){
					$row['direction1'] = $row['direction1'] + 180;
				}
				else {
					$row['direction1'] = $row['direction1'] -180;
				}
			}

			// statusが0でなければその値を使用する
			if( $row['status'] > 0)
			{
				$row['TS'] = $row['status'];
			}
			$pos_data = array(
				'mid'		=>	$row['mid'],
				'TS' 		=>	$row['TS'],
				'x'			=>	$x_row,
				'y'			=>	$y_row,
	      	// 'z'			=>	$row['z'],
	      		'color'		=>	$row['color'],
				'd1'		=>	$row['direction1'],
	      	// 'd2'		=>	$row['direction2'],
	      	// 'd3'		=>	$row['direction3'],
	      	// 'status'	=>	$row['status']
				);
			$data[] = $pos_data;
			if($cnt > $pack_num)
			{
				$cnt = 0;
				$index++;
			}
		}
		if($json == 1)
		{
			if(isset($_GET['pack_id']))
			{
				$json_data = array('Items' => $data);
				echo json_encode($json_data);
			}
			else
			{
				echo $index;
			}
		}
		else
		{
			echo json_encode($data);
		}

    // $str .= "]";
    // echo ob_get_length();
		header("Content-Length: " .ob_get_length());
	// echo $str;
	}
	mysql_close($link) or die("MySQL切断に失敗しました。");


	?>