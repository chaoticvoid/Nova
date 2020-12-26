// -----------------------------------------------------------------------
// <copyright file="Operation.cs" Company="Michael Tindal">
// Copyright 2011-2013 Michael Tindal
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace Nova.Runtime {
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public static class Operation {
        public static Expression Resolve(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Resolve", expectedType, args);
        }

        public static Expression ResolveSymbol(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("ResolveSymbol", expectedType, args);
        }

        public static Expression Assign(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Assign", expectedType, args);
        }

        public static Expression ConditionalAssign(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("ConditionalAssign", expectedType, args);
        }

        public static Expression ParallelAssign(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("ParallelAssign", expectedType, args);
        }

        public static Expression Binary(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Binary", expectedType, args);
        }

        public static Expression Compare(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Compare", expectedType, args);
        }

        public static Expression Match(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("Match", expectedType, args);
        }

        public static Expression Unary(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Unary", expectedType, args);
        }

        public static Expression Boolean(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Boolean", expectedType, args);
        }

        public static Expression Convert(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Convert", expectedType, args);
        }

        public static Expression Access(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Access", expectedType, args);
        }

        public static Expression AccessSet(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("AccessSet", expectedType, args);
        }

        public static Expression ConditionalAccessSet(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("ConditionalAccessSet", expectedType, args);
        }

        public static Expression CreateArray(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("CreateArray", expectedType, args);
        }

        public static Expression CreateDictionary(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("CreateDictionary", expectedType, args);
        }

        public static Expression KeyValuePair(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("KeyValuePair", expectedType, args);
        }

        public static Expression Define(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Define", expectedType, args);
        }

        public static Expression SingletonDefine(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("SingletonDefine", expectedType, args);
        }

        public static Expression Call(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Call", expectedType, args);
        }

        public static Expression Yield(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Yield", expectedType, args);
        }

        public static Expression String(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("String", expectedType, args);
        }

        public static Expression Regex(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("Regex", expectedType, args);
        }

        public static Expression Number(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("Number", expectedType, args);
        }

        public static Expression Invoke(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Invoke", expectedType, args);
        }

        public static Expression Eval(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Eval", expectedType, args);
        }

        public static Expression Alias(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Alias", expectedType, args);
        }

        public static Expression InstanceRef(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("InstanceRef", expectedType, args);
        }

        public static Expression Typeof(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Typeof", expectedType, args);
        }

        public static Expression Include(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Include", expectedType, args);
        }

        public static Expression DefineClass(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("DefineClass", expectedType, args);
        }

        public static Expression DefineClassOpen(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("DefineClassOpen", expectedType, args);
        }

        public static Expression DefineModule(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("DefineModule", expectedType, args);
        }

        public static Expression Throw(Type expectedType, params Expression[] args) {
            return RuntimeOperations.Op("Throw", expectedType, args);
        }

        public static Expression Begin(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("Begin", expectedType, args);
        }

        public static Expression Sync(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("Sync", expectedType, args);
        }

        public static Expression MethodChange(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("MethodChange", expectedType, args);
        }

        public static Expression ObjectMethodChange(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("ObjectMethodChange", expectedType, args);
        }

        public static Expression ConvertIfNumber(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("ConvertIfNumber", expectedType, args);
        }

        public static Expression Switch(Type expectedType, params Expression[] args)
        {
            return RuntimeOperations.Op("Switch", expectedType, args);
        }
    }
}