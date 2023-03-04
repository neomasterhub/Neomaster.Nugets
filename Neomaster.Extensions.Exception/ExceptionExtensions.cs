using System;
using System.Collections.Generic;
using System.Linq;
using E = System.Exception;

namespace Neomaster.Extensions.Exception
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Retrieves all inner exceptions of the <paramref name="e"/> tree, including <see cref="AggregateException"/>.
        /// </summary>
        /// <param name="e">The root of the exception tree, which are returned as a list.</param>
        /// <param name="addAggregate">If <c>true</c>, <see cref="AggregateException"/> nodes will be included, default is <c>false</c>.</param>
        /// <param name="aggregateToSingle">If <c>true</c>, <see cref="E"/> exception will be included with <see cref="AggregateException.Message"/> instead of <see cref="AggregateException"/> node.</param>
        /// <returns>List of all the inner exceptions of the <paramref name="e"/> tree.</returns>
        public static List<E> GetAllInnerExceptions(this E e, bool addAggregate = false, bool aggregateToSingle = false)
        {
            List<E> list = new List<E>();

            if (e == null)
            {
                return list;
            }

            if (e is AggregateException ae)
            {
                if (addAggregate)
                {
                    if (aggregateToSingle)
                    {
                        list.Add(new E(ae.Message));
                    }
                    else
                    {
                        list.Add(ae);
                    }
                }

                list.AddRange(ae.InnerExceptions.SelectMany(ie => ie.GetAllInnerExceptions(addAggregate, aggregateToSingle)));
            }
            else
            {
                list.Add(e);
                list.AddRange(e.InnerException.GetAllInnerExceptions(addAggregate, aggregateToSingle));
            }

            return list;
        }
    }
}
