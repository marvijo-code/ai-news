import React, { useState, useEffect } from 'react';
import { Container, Typography, Card, CardContent, Grid, CircularProgress, Button } from '@mui/material';
import axios from 'axios';
import EmailShareModal from './components/EmailShareModal';

interface NewsItem {
  title: string;
  description: string;
  url: string;
  publishedAt: string;
}

const App: React.FC = () => {
  const [news, setNews] = useState<NewsItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [shareModalOpen, setShareModalOpen] = useState(false);
  const [selectedNewsItem, setSelectedNewsItem] = useState<NewsItem | null>(null);

  useEffect(() => {
    const fetchNews = async () => {
      try {
        const response = await axios.get<NewsItem[]>('http://localhost:5000/api/news');
        setNews(response.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching news:', error);
        setLoading(false);
      }
    };

    fetchNews();
  }, []);

  const handleShareClick = (item: NewsItem) => {
    setSelectedNewsItem(item);
    setShareModalOpen(true);
  };

  return (
    <Container maxWidth="md">
      <Typography variant="h2" component="h1" gutterBottom>
        AI News
      </Typography>
      {loading ? (
        <CircularProgress />
      ) : (
        <Grid container spacing={3}>
          {news.map((item, index) => (
            <Grid item xs={12} key={index}>
              <Card>
                <CardContent>
                  <Typography variant="h5" component="h2">
                    {item.title}
                  </Typography>
                  <Typography color="textSecondary" gutterBottom>
                    {new Date(item.publishedAt).toLocaleString()}
                  </Typography>
                  <Typography variant="body2" component="p">
                    {item.description}
                  </Typography>
                  <Typography variant="body2" component="p">
                    <a href={item.url} target="_blank" rel="noopener noreferrer">
                      Read more
                    </a>
                  </Typography>
                  <Button
                    variant="contained"
                    color="primary"
                    onClick={() => handleShareClick(item)}
                    sx={{ mt: 2 }}
                  >
                    Share via Email
                  </Button>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
      {selectedNewsItem && (
        <EmailShareModal
          open={shareModalOpen}
          onClose={() => setShareModalOpen(false)}
          newsItem={selectedNewsItem}
        />
      )}
    </Container>
  );
};

export default App;
