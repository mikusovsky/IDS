using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntervalTree;
using Wintellect.PowerCollections;

namespace IDS.IDS.IntervalTree
{
   /// <summary>
   /// The Node class contains the interval tree information for one single node
   /// </summary>
   public class IntervalNode<T, D> where D : struct, IComparable<D>
   {
      private OrderedDictionary<Interval<T, D>, List<Interval<T, D>>> m_Intervals;
      private D m_Center;
      private IntervalNode<T, D> m_LeftNode;
      private IntervalNode<T, D> m_RightNode;

      public IntervalNode()
      {
         m_Intervals = new OrderedDictionary<Interval<T, D>, List<Interval<T, D>>>();
         m_Center = default(D);
         m_LeftNode = null;
         m_RightNode = null;
      }

      private string debug
      {
         get
         {
            StringBuilder sb = new StringBuilder();
            foreach (var key in m_Intervals.Keys)
            {
               sb.AppendLine(key.ToString());
               sb.AppendLine("==");
               foreach (var val in m_Intervals[key])
               {
                  sb.AppendLine(val.ToString());
                  sb.AppendLine("--");
               }

               sb.AppendLine("***");
            }

            return sb.ToString();
         }
      }

      public IntervalNode(List<Interval<T, D>> intervalList)
      {

         m_Intervals = new OrderedDictionary<Interval<T, D>, List<Interval<T, D>>>();

         var endpoints = new OrderedSet<D>();

         foreach (var interval in intervalList)
         {
            endpoints.Add(interval.Start);
            endpoints.Add(interval.End);
         }

         Nullable<D> median = GetMedian(endpoints);
         m_Center = median.GetValueOrDefault();

         List<Interval<T, D>> left = new List<Interval<T, D>>();
         List<Interval<T, D>> right = new List<Interval<T, D>>();

         foreach (Interval<T, D> interval in intervalList)
         {
            if (interval.End.CompareTo(m_Center) < 0)
               left.Add(interval);
            else if (interval.Start.CompareTo(m_Center) > 0)
               right.Add(interval);
            else
            {
               List<Interval<T, D>> posting;
               if (!m_Intervals.TryGetValue(interval, out posting))
               {
                  posting = new List<Interval<T, D>>();
                  m_Intervals.Add(interval, posting);
               }
               posting.Add(interval);
            }
         }

         if (left.Count > 0)
            m_LeftNode = new IntervalNode<T, D>(left);
         if (right.Count > 0)
            m_RightNode = new IntervalNode<T, D>(right);
      }

      public IEnumerable<IList<Interval<T, D>>> Intersections
      {
         get
         {
            if (m_Intervals.Count == 0) yield break;
            else if (m_Intervals.Count == 1)
            {
               if (m_Intervals.First().Value.Count > 1)
               {
                  yield return m_Intervals.First().Value;
               }
            }
            else
            {
               var keys = m_Intervals.Keys.ToArray();

               int lastIntervalIndex = 0;
               List<Interval<T, D>> intersectionsKeys = new List<Interval<T, D>>();
               for (int index = 1; index < m_Intervals.Count; index++)
               {
                  var intervalKey = keys[index];
                  if (intervalKey.Intersects(keys[lastIntervalIndex]))
                  {
                     if (intersectionsKeys.Count == 0)
                     {
                        intersectionsKeys.Add(keys[lastIntervalIndex]);
                     }
                     intersectionsKeys.Add(intervalKey);
                  }
                  else
                  {
                     if (intersectionsKeys.Count > 0)
                     {
                        yield return GetIntervalsOfKeys(intersectionsKeys);
                        intersectionsKeys = new List<Interval<T, D>>();
                        index--;
                     }
                     else
                     {
                        if (m_Intervals[intervalKey].Count > 1)
                        {
                           yield return m_Intervals[intervalKey];
                        }
                     }

                     lastIntervalIndex = index;
                  }
               }

               if (intersectionsKeys.Count > 0) yield return GetIntervalsOfKeys(intersectionsKeys);
            }
         }
      }

      private List<Interval<T, D>> GetIntervalsOfKeys(List<Interval<T, D>> intervalKeys)
      {
         var allIntervals =
           from k in intervalKeys
           select m_Intervals[k];

         return allIntervals.SelectMany(x => x).ToList();
      }

      /// <summary>
      /// Perform a stabbing Query on the node
      /// </summary>
      /// <param name="time">the time to Query at</param>
      /// <returns>all stubedIntervals containing time</returns>
      public List<Interval<T, D>> Stab(D time, ContainConstrains constraint)
      {
         List<Interval<T, D>> result = new List<Interval<T, D>>();

         foreach (var entry in m_Intervals)
         {
            if (entry.Key.Contains(time, constraint))
               foreach (var interval in entry.Value)
                  result.Add(interval);
            else if (entry.Key.Start.CompareTo(time) > 0)
               break;
         }

         if (time.CompareTo(m_Center) < 0 && m_LeftNode != null)
            result.AddRange(m_LeftNode.Stab(time, constraint));
         else if (time.CompareTo(m_Center) > 0 && m_RightNode != null)
            result.AddRange(m_RightNode.Stab(time, constraint));
         return result;
      }

      /// <summary>
      /// Perform an interval intersection Query on the node
      /// </summary>
      /// <param name="target">the interval to intersect</param>
      /// <returns>all stubedIntervals containing time</returns>
      public List<Interval<T, D>> Query(Interval<T, D> target)
      {
         List<Interval<T, D>> result = new List<Interval<T, D>>();

         foreach (var entry in m_Intervals)
         {
            if (entry.Key.Intersects(target))
               foreach (Interval<T, D> interval in entry.Value)
                  result.Add(interval);
            else if (entry.Key.Start.CompareTo(target.End) > 0)
               break;
         }

         if (target.Start.CompareTo(m_Center) < 0 && m_LeftNode != null)
            result.AddRange(m_LeftNode.Query(target));
         if (target.End.CompareTo(m_Center) > 0 && m_RightNode != null)
            result.AddRange(m_RightNode.Query(target));
         return result;
      }

      public D Center
      {
         get { return m_Center; }
         set { m_Center = value; }
      }

      public IntervalNode<T, D> Left
      {
         get { return m_LeftNode; }
         set { m_LeftNode = value; }
      }

      public IntervalNode<T, D> Right
      {
         get { return m_RightNode; }
         set { m_RightNode = value; }
      }

      /// <summary>
      /// the median of the set, not interpolated
      /// </summary>
      /// <param name="set"></param>
      /// <returns></returns>
      private Nullable<D> GetMedian(OrderedSet<D> set)
      {
         int i = 0;
         int middle = set.Count / 2;
         foreach (D point in set)
         {
            if (i == middle)
               return point;
            i++;
         }
         return null;
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         sb.Append(m_Center + ": ");
         foreach (var entry in m_Intervals)
         {
            sb.Append("[" + entry.Key.Start + "," + entry.Key.End + "]:{");
            foreach (Interval<T, D> interval in entry.Value)
            {
               sb.Append("(" + interval.Start + "," + interval.End + "," + interval.Data + ")");
            }
            sb.Append("} ");
         }
         return sb.ToString();
      }

   }
}