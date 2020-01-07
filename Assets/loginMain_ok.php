<?php
	include "./db.php" ;

	$userId = $_POST['userID'];
	$password = $_POST['userPW'];

	//$connect = mysql_connect("192.168.1.183","test", "kssikssi") or die("connect fail!".mysqli_error());
	//if(!connect) die("connect fail!".mysqli_error());

	$sql = mq("select * from member where id='".$_POST['userid']."'");

    $member = $sql->fetch_array();
    $hash_pw = $member['pw'];

	if(password_verify($password, $hash_pw)){   //password_verify(비교할문자, $hash값) : password가 hash와 맞는지 확인하는 함수, password_hash()로 생성된 해시값이 맞는지 확인함        
		$_SESSION['userid'] = $member['id'];
        $_SESSION['userpw'] = $member['pw'];
		
		//echo("testtest1");
        //echo "<script> alert('로그인 되었습니다.'); location.href='../home/index.php'; </script>";   
    }
    else{
        //echo "<script> alert('아이디 혹은 비밀번호를 확인하세요'); history.back(); </script>";
		//echo("testtest2");
    }

	/*
    session_start();
    include "../common/db.php" ;

    $password = $_POST['userpw'];
    $sql = mq("select * from member where id='".$_POST['userid']."'");

    $member = $sql->fetch_array();
    $hash_pw = $member['pw'];

    if(password_verify($password, $hash_pw)){   //password_verify(비교할문자, $hash값) : password가 hash와 맞는지 확인하는 함수, password_hash()로 생성된 해시값이 맞는지 확인함
        $_SESSION['userid'] = $member['id'];
        $_SESSION['userpw'] = $member['pw'];

        echo "<script> alert('로그인 되었습니다.'); location.href='../home/index.php'; </script>";   
    }
    else{
        echo "<script> alert('아이디 혹은 비밀번호를 확인하세요'); history.back(); </script>";
    }
	*/
?>

