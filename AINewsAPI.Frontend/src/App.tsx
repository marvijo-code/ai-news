import React, { useState, useEffect } from 'react';
import { Container, Typography, Card, CardContent, Grid, CircularProgress } from '@mui/material';
import axios from 'axios';

interface NewsItem {
  title: string;
  description: string;
  url: string;
  publishedAt: string;
}

const App: React.FC = () => {
  const [news, setNews] = useState<NewsItem[]>([]);
  const [loading, setLoading] = useState(true);

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
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
    </Container>
  );
};

export default App;
