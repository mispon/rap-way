namespace UI.Controls.ScrollViewController
{
    public interface IScrollViewControllerItem
    {
        void SetPosition(float spacing);
        float GetHeight();
        float GetWidth();
    }
}