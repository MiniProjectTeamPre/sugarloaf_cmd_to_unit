using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sugarloaf_cmd_to_unit {
    class Version_Update {
        //V2022.02
        //Add show detail temp fail

        //V2022.03
        //Edit time befor read serial from 100 to 150 mS

        //V2022.04
        //Add log temp
        //Edit bug read temp to cancel skip rx "13" in receive

        //V2023.01
        //Add log program catch

        //V2023.02
        //Edit image icon

        //V2023.03
        //พยายามจะแก้บัค ที่โปรแกรมมันแคลช ชอบปิดตังเอง ประเด็นคือมันไม่บอกว่าแคลชบรรทัดไหน

        //V2023.04
        //แก้บัควัด bat ไม่ได้ ต้องใส่ tx >= 1 ตอนแรก tx > 1

        //V2023.05
        //แก้บัค Event DataReceived เพิ่มให้ set flag เฉพาะตอนที่มี data ใน tx เท่านั้น ไม่งั้นมันจะไปแคลชที่อื่น
    }
}
