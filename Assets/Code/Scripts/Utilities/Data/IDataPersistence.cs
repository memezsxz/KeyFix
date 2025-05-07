internal interface IDataPersistence
{
    void SaveData(ref SaveData data);
    void LoadData(ref SaveData data);
}