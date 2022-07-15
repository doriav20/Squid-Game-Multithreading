namespace SquidGameMultithreading
{
    public class Person
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int FaceImageID { get; set; }
        public int Age { get; set; }
        public Person(string id, string name, int faceID, int age)
        {
            this.ID = id;
            this.Name = name;
            this.FaceImageID = faceID;
            this.Age = age;
        }
        public override bool Equals(object obj)
        {
            Person p = obj as Person;
            if (p == null) return false;
            return this.ID == p.ID;
        }
    }
}
