using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NServiceBus.MessageInterfaces;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Serialization;
using System.Runtime.Serialization;

namespace NServiceBus.Serializers.InterfacesToXML.Test
{
    public class Class1
    {
        private int number = 50;
        private int numberOfIterations = 1000;

        public void Test()
        {
            Debug.WriteLine("Interfaces");
            TestInterfaces();

            Debug.WriteLine("XML");
            TestXml();
        }

        public void TestInterfaces()
        {
            IMessageMapper mapper = new MessageMapper();
            MessageSerializer serializer = new MessageSerializer();
            serializer.MessageMapper = mapper;

            serializer.Initialize(typeof(IM2), typeof(IM1));

            IM2 o = mapper.CreateInstance<IM2>();

            o.Id = Guid.NewGuid();
            o.Age = 10;
            o.Address = Guid.NewGuid().ToString();
            o.Int = 7;
            o.Name = "udi";
            o.Risk = new Risk(0.15D, true);
            o.Some = SomeEnum.B;

            o.Parent = mapper.CreateInstance<IM1>();
            o.Parent.Name = "udi";
            o.Parent.Age = 10;
            o.Parent.Address = Guid.NewGuid().ToString();
            o.Parent.Int = 7;
            o.Parent.Name = "-1";
            o.Parent.Risk = new Risk(0.15D, true);

            o.Names = new List<IM1>();
            for (int i = 0; i < number; i++)
            {
                IM1 m1 = mapper.CreateInstance<IM1>();
                o.Names.Add(m1);
                m1.Age = 10;
                m1.Address = Guid.NewGuid().ToString();
                m1.Int = 7;
                m1.Name = i.ToString();
                m1.Risk = new Risk(0.15D, true);
            }

            IMessage[] messages = new IMessage[] {o};

            Time(messages, serializer);
        }

        public void TestXml()
        {
            IMessageSerializer serializer = new XML.MessageSerializer();

            serializer.Initialize(typeof(M2), typeof(M1));

            M2 o = new M2();
            o.Id = Guid.NewGuid();
            o.Age = 10;
            o.Address = Guid.NewGuid().ToString();
            o.Int = 7;
            o.Name = "udi";
            o.Risk = new Risk(0.15D, true);
            o.Some = SomeEnum.B;

            o.Parent = new M1();
            o.Parent.Name = "udi";
            o.Parent.Age = 10;
            o.Parent.Address = Guid.NewGuid().ToString();
            o.Parent.Int = 7;
            o.Parent.Name = "-1";
            o.Parent.Risk = new Risk(0.15D, true);

            o.Names = new List<M1>();
            for (int i = 0; i < number; i++)
            {
                M1 m1 = new M1();
                o.Names.Add(m1);
                m1.Age = 10;
                m1.Address = Guid.NewGuid().ToString();
                m1.Int = 7;
                m1.Name = i.ToString();
                m1.Risk = new Risk(0.15D, true);
            }

            IMessage[] messages = new IMessage[] { o };

            Time(messages, serializer);
        }

        private void Time(IMessage[] messages, IMessageSerializer serializer)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < numberOfIterations; i++)
            {
                MemoryStream stream = new MemoryStream();
                serializer.Serialize(messages, stream);

                stream.Close();
            }

            watch.Stop();
            Debug.WriteLine("Serializing: " + watch.Elapsed);

            watch.Reset();

            MemoryStream s = new MemoryStream();
            serializer.Serialize(messages, s);
            byte[] buffer = s.GetBuffer();
            s.Dispose();

            watch.Start();

            for (int i = 0; i < numberOfIterations; i++)
            {

                MemoryStream forDeserializing = new MemoryStream(buffer);

                object result = serializer.Deserialize(forDeserializing);
                forDeserializing.Close();
            }

            watch.Stop();
            Debug.WriteLine("Deserializing: " + watch.Elapsed);
        }

    }

    public interface IM1 : IMessage
    {
        int Age { get; set; }
        int Int { get; set; }
        string Name { get; set; }
        string Address { get; set; }
        Risk Risk { get; set; }
    }

    public interface IM2 : IMessage
    {
        int Age { get; set; }
        Guid Id { get; set; }
        int Int { get; set; }
        string Name { get; set; }
        string Address { get; set; }
        Risk Risk { get; set; }
        IM1 Parent { get; set; }
        List<IM1> Names { get; set; }
        SomeEnum Some { get; set; }
    }

    [Serializable]
    public class M2 : M1, ISerializable
    {
        private Guid id;
        private List<M1> names;
        private M1 parent;
        private SomeEnum some;

        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public List<M1> Names
        {
            get { return names; }
            set { names = value; }
        }

        public M1 Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public SomeEnum Some
        {
            get { return some; }
            set { some = value; }
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", id);
            info.AddValue("some", some);
            info.AddValue("parent", parent);
            info.AddValue("names", names);

            base.GetObjectData(info, context);
        }

        public M2(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            id = (Guid)info.GetValue("id", typeof (Guid));
            some = (SomeEnum) info.GetValue("some", typeof (SomeEnum));
            parent = (M1) info.GetValue("parent", typeof (M1));
            names = (List<M1>) info.GetValue("names", typeof (List<M1>));
        }

        public M2()
        {
            
        }

        #endregion
    }

    [Serializable]
    public class M1 : IMessage, ISerializable
    {
        private int age;
        private int intt;
        private string name;
        private string address;
        private Risk risk;

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public int Int
        {
            get { return intt; }
            set { intt = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public Risk Risk
        {
            get { return risk; }
            set { risk = value; }
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("address", address);
            info.AddValue("age", age);
            info.AddValue("intt", intt);
            info.AddValue("name", name);
            info.AddValue("risk", risk);
        }

        public M1(SerializationInfo info, StreamingContext context)
        {
            address = info.GetString("address");
            age = info.GetInt32("age");
            intt = info.GetInt32("intt");
            name = info.GetString("name");
            risk = (Risk)info.GetValue("risk", typeof (Risk));
        }

        public M1()
        {
            
        }

        #endregion
    }

    [Serializable]
    public class Risk
    {
        public Risk() { }
        public Risk(double percent, bool annnum)
        {
            this.percent = percent;
            this.annum = annnum;
        }

        private double percent;

        public bool Annum
        {
            get { return annum; }
            set { annum = value; }
        }

        public double Percent
        {
            get { return percent; }
            set { percent = value; }
        }

        private bool annum;
    }

    public enum SomeEnum
    {
        A,
        B
    }
}
