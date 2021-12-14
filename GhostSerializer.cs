using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;
using UnityEngine;

public class GhostSerializer : MonoBehaviour {

    public static void SerializeObject(LevelProgressManager.SnapShot[] serializableObject, string fileName)
    {
        XmlDocument xmlDocument = new XmlDocument();
        XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
        using (MemoryStream stream = new MemoryStream())
        {
            serializer.Serialize(stream, serializableObject);
            stream.Position = 0;
            xmlDocument.Load(stream);
            xmlDocument.Save(fileName);
            stream.Close();
        }
    }

    public static LevelProgressManager.SnapShot[] DeSerializeObject(string fileName)
    {
        LevelProgressManager.SnapShot[] objectOut;

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(fileName);
        string xmlString = xmlDocument.OuterXml;

        using (StringReader read = new StringReader(xmlString))
        {
            Type outType = typeof(LevelProgressManager.SnapShot[]);

            XmlSerializer serializer = new XmlSerializer(outType);
            using (XmlReader reader = new XmlTextReader(read))
            {
                objectOut = (LevelProgressManager.SnapShot[])serializer.Deserialize(reader);
                reader.Close();
            }

            read.Close();
        }
        return objectOut;
    }
}
