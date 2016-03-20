﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TH_Global.Functions
{
    public static class Object_Functions
    {

        public delegate T ObjectActivator<T>(params object[] args);

        public  static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            var param = System.Linq.Expressions.Expression.Parameter(typeof(object[]), "args");

            var argsExp = new System.Linq.Expressions.Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                var index = System.Linq.Expressions.Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                var paramAccessorExp = System.Linq.Expressions.Expression.ArrayIndex(param, index);

                var paramCastExp = System.Linq.Expressions.Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            var newExp = System.Linq.Expressions.Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            var lambda = System.Linq.Expressions.Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            //compile it
            ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled;
        }



    }
}