using System;

namespace IntervalTree
{
   /// <summary>
   /// The Interval class maintains an interval with some associated data
   /// </summary>
   /// <typeparam name="T">The type of data being stored</typeparam>
   public class Interval<T, D> : IComparable<Interval<T, D>> where D : IComparable<D>
   {

      private D m_Start;
      private D m_End;
      private T m_Data;

      public Interval(D start, D end, T data)
      {
         m_Start = start;
         m_End = end;
         m_Data = data;
      }

      public D Start
      {
         get { return m_Start; }
         set { m_Start = value; }
      }

      public D End
      {
         get { return m_End; }
         set { m_End = value; }
      }

      public T Data
      {
         get { return m_Data; }
         set { m_Data = value; }
      }

      public bool Contains(D time, ContainConstrains constraint)
      {
         bool isContained;

         switch (constraint)
         {
            case ContainConstrains.None:
               isContained = Contains(time);
               break;
            case ContainConstrains.IncludeStart:
               isContained = ContainsWithStart(time);
               break;
            case ContainConstrains.IncludeEnd:
               isContained = ContainsWithEnd(time);
               break;
            case ContainConstrains.IncludeStartAndEnd:
               isContained = ContainsWithStartEnd(time);
               break;
            default:
               throw new ArgumentException("Ivnalid constraint " + constraint);
         }

         return isContained;
      }

      /// <summary>
      /// true if this interval contains time (inclusive)
      /// </summary>
      /// <param name="time"></param>
      /// <returns></returns>
      public bool Contains(D time)
      {
         //return time < end && time > start;
         return time.CompareTo(m_End) < 0 && time.CompareTo(m_Start) > 0;
      }

      /// <summary>
      /// true if this interval contains time (including start).
      /// </summary>
      /// <param name="time"></param>
      /// <returns></returns>
      public bool ContainsWithStart(D time)
      {
         return time.CompareTo(m_End) < 0 && time.CompareTo(m_Start) >= 0;
      }

      /// <summary>
      /// true if this interval contains time (including end).
      /// </summary>
      /// <param name="time"></param>
      /// <returns></returns>
      public bool ContainsWithEnd(D time)
      {
         return time.CompareTo(m_End) <= 0 && time.CompareTo(m_Start) > 0;
      }

      /// <summary>
      /// true if this interval contains time (include start and end).
      /// </summary>
      /// <param name="time"></param>
      /// <returns></returns>
      public bool ContainsWithStartEnd(D time)
      {
         return time.CompareTo(m_End) <= 0 && time.CompareTo(m_Start) >= 0;
      }

      /// <summary>
      /// return true if this interval intersects other
      /// </summary>
      /// <param name="?"></param>
      /// <returns></returns>
      public bool Intersects(Interval<T, D> other)
      {
         //return other.End > start && other.Start < end;
         return other.End.CompareTo(m_Start) > 0 && other.Start.CompareTo(m_End) < 0;
      }


      /// <summary>
      /// Return -1 if this interval's start time is less than the other, 1 if greater
      /// In the event of a tie, -1 if this interval's end time is less than the other, 1 if greater, 0 if same
      /// </summary>
      /// <param name="other"></param>
      /// <returns></returns>
      public int CompareTo(Interval<T, D> other)
      {
         if (m_Start.CompareTo(other.Start) < 0)
            return -1;
         else if (m_Start.CompareTo(other.Start) > 0)
            return 1;
         else if (m_End.CompareTo(other.End) < 0)
            return -1;
         else if (m_End.CompareTo(other.End) > 0)
            return 1;
         else
            return 0;
         //if (start < other.Start)
         //  return -1;
         //else if (start > other.Start)
         //  return 1;
         //else if (end < other.End)
         //  return -1;
         //else if (end > other.End)
         //  return 1;
         //else
         //  return 0;
      }

      public override string ToString()
      {
         return string.Format("{0}-{1}", m_Start, m_End);
      }
   }
}

public enum ContainConstrains
{
   None,
   IncludeStart,
   IncludeEnd,
   IncludeStartAndEnd
}