using System;
using FluentAssertions;
using TheFlow.CoreConcepts;
using Xunit;

namespace TheFlow.Tests.Unit
{
    public class TokenShould
    {
        [Fact]
        public void InitializeWithNonEmptyGuid()
        {
            var token = Token.Create();
            token.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void BeActiveByDefault()
        {
            var token = Token.Create();
            token.IsActive.Should().BeTrue();
        }
        
        [Fact]
        public void ReturnNewTokenWhenCallingAllocateChild()
        {
            var parent = Token.Create();
            var child = parent.AllocateChild();
            child.Should().NotBeNull();
        }

        [Fact]
        public void SetParentPropertyWhenCallingAllocateChild()
        {
            var parent = Token.Create();
            var child = parent.AllocateChild();
            child.ParentId.Should().Be(parent.Id);
        }

        [Fact]
        public void SetExecutionPointOfChildAsTheSameOfTheParent()
        {
            var parent = Token.Create();
            parent.ExecutionPoint = "newPos";
            var child = parent.AllocateChild();
            child.ExecutionPoint.Should().Be(parent.ExecutionPoint);
        }

        [Fact]
        public void HaveAListOfChildren()
        {
            var parent = Token.Create();
            var child1 = parent.AllocateChild();
            var child2 = parent.AllocateChild();
            parent.Children.Should().BeEquivalentTo(
                child1, child2);
        }

        [Fact]
        public void BeMarkedAsReleasedAfterTheExecutionOfTheReleaseMethod()
        {
            var token = Token.Create();
            token.WasReleased.Should().Be(false);
            token.Release();
            token.WasReleased.Should().Be(true);
        }

        [Fact]
        public void BeRemovedFromLiveDescendantsWhenRelased()
        {
            var parent = Token.Create();
            var child1 = parent.AllocateChild();
            var child2 = parent.AllocateChild();
            child1.Release();
            parent.GetActiveDescendants().Should().BeEquivalentTo(
                child2);
        }

        [Fact]
        public void FailToAllocateChildAfterRelased()
        {
            var token = Token.Create();
            token.Release();
            token.Invoking(t => t.AllocateChild())
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EnumerateAllDescendants()
        {
            var token = Token.Create();
            var child1 = token.AllocateChild();
            var child2 = token.AllocateChild();
            var granchild = child1.AllocateChild();
            
            token.GetDescendants().Should().BeEquivalentTo(
                child1, granchild, child2);
        }

        [Fact]
        public void BeAbleToFindById()
        {
            var token = Token.Create();
            var child1 = token.AllocateChild();
            var child2 = token.AllocateChild();
            var granchild = child1.AllocateChild();
            
            token.GetDescendants().Should().BeEquivalentTo(
                child1, granchild, child2);

            token.FindById(granchild.Id).Should().Be(granchild);
            token.FindById(child1.Id).Should().Be(child1);
            token.FindById(child2.Id).Should().Be(child2);
            token.FindById(token.Id).Should().Be(token);
        }

        [Fact]
        public void ReturnNullFindingAnInexistentId()
        {
            var token = Token.Create();
            token.FindById(Guid.NewGuid()).Should().BeNull();
        }
        
    }
}