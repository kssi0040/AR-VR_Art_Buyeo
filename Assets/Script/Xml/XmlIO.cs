using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class XmlIO
{
    static XmlDocument xmlDoc = new XmlDocument();

    public static T LoadXml<T>(string FilePath) where T:class
    {
        Debug.Log(FilePath);
        //XML 파일 내부의 문자를 전부 읽어드림
        string xmlFile = File.ReadAllText(FilePath);
        
        xmlDoc.LoadXml(xmlFile);
        
        using (var stream = new StringReader(xmlFile))
        {
            //템플릿에 정해진 구조대로 변수를 생성하고 저장
            var s = new XmlSerializer(typeof(T));
            return s.Deserialize(stream) as T;
        }
    }
}
