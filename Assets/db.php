<?php

        session_start();    
        header('Content-type: text/html; charset=utf-8');   //utf-8 인코딩
    
        // $db = new mysqli("localhost", "root", "kssikssi", "test1");  //DB호스트주소, DB아이디, DB암호, DB이름(테이블이름X)
        $db = new mysqli("192.168.1.183", "test", "kssikssi", "arvr_web2");      //windows 안에 저장한 mysql DB 연결
        $db -> set_charset("utf8");
        
        function mq($sql)
        {
               global $db;  //global로 외부에서 선언된 $sql를 함수내에서 쓸 수 있도록 해줌
                return $db -> query($sql);
        }

?>