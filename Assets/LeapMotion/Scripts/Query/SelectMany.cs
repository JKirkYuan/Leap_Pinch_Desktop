/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using System;

namespace Leap.Unity.Query {

  public struct SelectManyOp<SourceType, ResultType, SourceOp, ResultOp> : IQueryOp<ResultType>
    where SourceOp : IQueryOp<SourceType>
    where ResultOp : IQueryOp<ResultType> {

    private SourceOp _source;
    private Func<SourceType, QueryWrapper<ResultType, ResultOp>> _selector;

    private bool _hasInner;
    private ResultOp _innerSource;

    public SelectManyOp(SourceOp source, Func<SourceType, QueryWrapper<ResultType, ResultOp>> selector) {
      _source = source;
      _selector = selector;

      SourceType st;
      if (_source.TryGetNext(out st)) {
        _innerSource = _selector(st).op;
        _hasInner = true;
      } else {
        _innerSource = default(ResultOp);
        _hasInner = false;
      }
    }

    public bool TryGetNext(out ResultType t) {
      if (!_hasInner) {
        t = default(ResultType);
        return false;
      }

      while (!_innerSource.TryGetNext(out t)) {
        SourceType st;
        if (!_source.TryGetNext(out st)) {
          _hasInner = false;
          return false;
        }
        _innerSource = _selector(st).op;
      }
      return true;
    }

    public void Reset() {
      _source.Reset();

      SourceType st;
      if (_source.TryGetNext(out st)) {
        _innerSource = _selector(st).op;
        _hasInner = true;
      } else {
        _innerSource = default(ResultOp);
        _hasInner = false;
      }
    }
  }

  public partial struct QueryWrapper<QueryType, QueryOp> where QueryOp : IQueryOp<QueryType> {
    public QueryWrapper<NewType, SelectManyOp<QueryType, NewType, QueryOp, NewOp>> SelectMany<NewType, NewOp>(Func<QueryType, QueryWrapper<NewType, NewOp>> selector)
      where NewOp : IQueryOp<NewType> {
      return new QueryWrapper<NewType, SelectManyOp<QueryType, NewType, QueryOp, NewOp>>(new SelectManyOp<QueryType, NewType, QueryOp, NewOp>(_op, selector));
    }
  }
}
