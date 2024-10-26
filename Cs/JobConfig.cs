///程序生成文件,请勿自行更改!!!
///文件生成时间-2024/10/26 10:20:52
using System.Collections.Generic;
public class JobConfig
{
    public string ID {get;set;}
    public JobType Name {get;set;}
    public string Description {get;set;}
    public JobConfig(string ID ,JobType Name ,string Description )
    {
        this.ID = ID;
        this.Name = Name;
        this.Description = Description;
    }

    public JobConfig(){
    
    }
}