using EKanbanWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Models.api
{
    public class KanbanFactory
    {
        private EKanbanWebDBContext DbContext { get; set; }

        static object locker = new object();
        public static Dictionary<string, Tuple<DateTime, List<KanbanItem>>> ReqItems = new Dictionary<string, Tuple<DateTime, List<KanbanItem>>>();

        public KanbanFactory(EKanbanWebDBContext context)
        {
            DbContext = context;
        }

        public void Initialize(string authorizationToken)
        {
            lock (locker)
            {
                List<KanbanRequest> reqList = DbContext.KanbanRequest.Where(a => a.StatusId == 3).ToList();
                List<KanbanItem> reqItems = new List<KanbanItem>();
                foreach (KanbanRequest request in reqList)
                {
                    List<KanbanReqItem> items = DbContext.KanbanReqItem.Where(a => a.KanbanReqId==request.KanbanReqId).ToList();
                    foreach(KanbanReqItem item in items)
                    {
                        MsPart part = DbContext.MsPart.Where(a => a.PartId == item.PartId).FirstOrDefault();
                        if (part != null)
                        {
                            KanbanItem kanbanItem = new KanbanItem();
                            kanbanItem.ReqItemId = item.ReqItemId;
                            kanbanItem.KanbanReqId = request.KanbanReqId;
                            kanbanItem.RequestDate = request.RequestDateTime.AddMilliseconds(-request.RequestDateTime.Millisecond);
                            kanbanItem.ReqNo = request.TagRequestNo;
                            kanbanItem.PartId = item.PartId;
                            kanbanItem.PartNo = part.PartNo;
                            kanbanItem.LotSize = part.LotSize;
                            kanbanItem.Zone = part.Zone;
                            kanbanItem.OrderQty = item.ReqKanban;
                            reqItems.Add(kanbanItem);
                        }
                    }
                }
                ReqItems.Add(authorizationToken,
                    Tuple.Create(DateTime.UtcNow.AddHours(1), reqItems));
            }
        }

        public static void ClearStaleData()
        {
            lock (locker)
            {
                var keys = ReqItems.Keys.ToList();
                foreach (var oneKey in keys)
                {
                    if (ReqItems.TryGetValue(oneKey, out Tuple<DateTime, List<KanbanItem>> result) &&
                        result.Item1 < DateTime.UtcNow)
                    {
                        ReqItems.Remove(oneKey);
                    }
                }
            }
        }
    }
}
